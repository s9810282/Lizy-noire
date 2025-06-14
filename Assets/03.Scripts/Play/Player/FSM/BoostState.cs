using UnityEngine;
using UnityEngine.Rendering;

public class BoostState : TimedState
{
    Vector3 boostDir;
    bool isExhaustion = false;
    int curCount = 0;

    private Vector3 targetforward = Vector3.zero;


    public BoostState(PlayerController player, float duration, float curDuration = 0f)
       : base(player, duration, curDuration)
    {
        boostDir = 
            player.InputDirection != Vector3.zero ? player.InputDirection : player.transform.forward;
    }

    public override void Enter()
    {
        base.Enter();

        EventBus.Subscribe<SlashHitEvent>(RecoverBoostTime);
        curCount = player.UpdateBoostCount(-1);

        isExhaustion = curCount == 0;

        if (CheckWall()) return;
        if (CheckBounce()) return;

        player.PlayerState = EPlayerState.Boost;
        player.AnimSetBool("isMove", true);
        player.AnimSetInt("KnockBack", 0);

        Vector3 nextPos = player.TargetPosition + boostDir;
        player.SetTargetPosition(nextPos);
        targetforward = player.GetPlayerToTargetFoward();
    }

    public override void Update()
    {
        base.Update();

        EventBus.Publish(new BoostUIEvent
        {
            boostCount = this.curCount,
            timer = this.timer,
            maxtimer = duration,
        });

        Debug.Log("Timer : " + this.timer + " maxTimer : " + duration);


        boostDir = player.InputDirection != Vector3.zero ? player.InputDirection : boostDir;

        CheckWall();
        CheckBounce();

        player.RotateTowardsDirection(targetforward);
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            player.TargetPosition,
            player.CurrentMoveSpeed * Time.deltaTime);


        if (Vector3.Distance(player.transform.position, player.TargetPosition) < 0.01f)
        {
            SetNextTargetPosition();
        }
    }

    public void SetNextTargetPosition()
    {
        player.RotateInstantly(targetforward);
        player.transform.position = player.TargetPosition;

        if (CheckWall()) return;

        Vector3 nextPos = player.transform.position + boostDir;
        player.SetTargetPosition(nextPos);
        targetforward = player.GetPlayerToTargetFoward();
    }

    public override void Exit()
    {
        player.UpdateBoostCount(0);
        player.ResetBoostTimer();
        EventBus.Unsubscribe<SlashHitEvent>(RecoverBoostTime);
    }

    public override void ExitToDefaultState()
    {
        player.FSMMachine.ChangeState(new MoveState(player, true));

        if (isExhaustion)
            player.AddEffect( new ExhaustionBuff("Å»Áø", 2f, player, EStatusEffect.Exhaustion, player.BaseMoveSpeed/2, 25, isEffect: false));
    }


    public bool CheckBounce()
    {
        if (player.RaycaseBounce(boostDir, out RaycastHit bounceHit))
        {
            if (bounceHit.collider.TryGetComponent(out IValueItem item))
            {
                player.UpdateUltValue((int)item.Execute());
            }
            else
            {
                Monster target = bounceHit.collider.GetComponent<Monster>();

                if (target != null)
                {
                    player.FSMMachine.ChangeState(new KnockbackState(player, -boostDir, target, timer, true));
                    return true;
                }
            }
        }

        return false;
    }
    public bool CheckWall()
    {
        if (player.RaycaseWall(boostDir, out RaycastHit wallHit))
        {
            player.SetTargetPosition(player.transform.position);
            player.RotateInstantly(boostDir);
            return true;
        }

        return false;
    }

    public void RecoverBoostTime(SlashHitEvent e)
    {
        Debug.Log("RecoveryBoost : " + timer);
        timer += e.revoverValue;

        if(timer > duration)
            timer = duration;
    }    
}
