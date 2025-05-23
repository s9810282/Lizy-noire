using UnityEngine;
using UnityEngine.Rendering;

public class BoostState : TimedState
{
    Vector3 boostDir;
    private Vector3 targetforward = Vector3.zero;

    public BoostState(PlayerController player, float duration, float damageValue = 0)
       : base(player, duration, damageValue)
    {
        boostDir = 
            player.InputDirection != Vector3.zero ? player.InputDirection : player.transform.forward;
    }

    public override void Enter()
    {
        base.Enter();

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
            player.RotateInstantly(targetforward);
            player.transform.position = player.TargetPosition;

            if (CheckWall()) return;
                 
            Vector3 nextPos = player.transform.position + boostDir;
            player.SetTargetPosition(nextPos);
            targetforward = player.GetPlayerToTargetFoward();
        }
    }

    public override void Exit()
    {

    }

    public override void ExitToDefaultState()
    {
        player.FSMMachine.ChangeState(new MoveState(player, true));
        player.AddEffect(
            new ExhaustionBuff("Å»Áø", 2f, player, EStatusEffect.Exhaustion, player.BaseMoveSpeed/2, 25));
    }


    public bool CheckBounce()
    {
        if (player.RaycaseBounce(boostDir, out RaycastHit bounceHit))
        {
            Monster target = bounceHit.collider.GetComponent<Monster>();

            if (target != null)
            {
                player.FSMMachine.ChangeState(new KnockbackState(player, -boostDir, target));
                return true;
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
}
