using UnityEngine;

public class MoveState : IState
{
    private PlayerController player;

    public MoveState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Vector3 nextPos = player.TargetPosition + player.InputDirection;

        if (player.CheckWall(player.InputDirection, out _))
        {
            player.RotateInstantly(player.InputDirection);
            player.ResetInput();
            player.FSMMachine.ChangeState(new IdleState(player));
            return;
        }

        if (player.CheckBounce(player.InputDirection, out RaycastHit bounceHit))
        {
            player.FSMMachine.ChangeState(new KnockbackState(player, -player.InputDirection));
            return;
        }

        player.SetTargetPosition(nextPos);
    }

    public void Update()
    {
        player.RotateTowardsDirection();

        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            player.TargetPosition,
            player.MoveSpeed * Time.deltaTime);


        if (Vector3.Distance(player.transform.position, player.TargetPosition) < 0.01f)
        {
            player.transform.position = player.TargetPosition;

            if (player.InputDirection != Vector3.zero)
            {
                player.FSMMachine.ChangeState(new MoveState(player));
            }
            else
            {
                player.ResetInput();
                player.FSMMachine.ChangeState(new IdleState(player));
            }
        }
    }

    public void Exit() { }
}
