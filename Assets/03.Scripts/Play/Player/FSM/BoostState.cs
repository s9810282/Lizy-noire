using UnityEngine;
using UnityEngine.Rendering;

public class BoostState : TimedState
{
    Vector3 boostDir;


    public BoostState(PlayerController player, float duration, float damageValue = 0)
       : base(player, duration, damageValue)
    {
        boostDir = 
            player.InputDirection != Vector3.zero ? player.InputDirection : player.transform.forward;
    }

    public override void Enter()
    {
        base.Enter();

        player.PlayerState = EPlayerState.Boost;
     
        Vector3 nextPos = player.TargetPosition + boostDir;
        player.SetTargetPosition(nextPos);

        if (player.CheckWall(boostDir, out _))
        {
            player.RotateInstantly(boostDir);
            player.ResetInput();
            return;
        }

        CheckBounce();
    }

    public override void Update()
    {
        base.Update();

        boostDir = player.InputDirection != Vector3.zero ? player.InputDirection : boostDir;

        if(CheckWall()) return;
        CheckBounce();

        player.RotateTowardsDirection();

        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            player.TargetPosition,
            player.BaseMoveSpeed * Time.deltaTime);


        if (Vector3.Distance(player.transform.position, player.TargetPosition) < 0.01f)
        {
            player.transform.position = player.TargetPosition;

            Vector3 nextPos = player.TargetPosition + boostDir;
            player.SetTargetPosition(nextPos);

            CheckBounce();
        }
    }

    public override void Exit()
    {

    }

    public override void ExitToDefaultState()
    {
        player.ResetInput();
        player.SetTargetPosition(player.transform.position);

        player.FSMMachine.ChangeState(new IdleState(player));
        player.AddEffect(new ExhaustionBuff("Å»Áø", 2f, player, EStatusEffect.Exhaustion, player.BaseMoveSpeed, 25));
    }


    public bool CheckBounce()
    {
        if (player.CheckBounce(boostDir, out RaycastHit bounceHit))
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
        if (player.CheckWall(boostDir, out _))
        {
            player.RotateInstantly(boostDir);
            player.ResetInput();
            return true;
        }

        return false;
    }
}
