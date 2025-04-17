using UnityEngine;

public class IdleState : IState
{
    private PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter() { }

    public void Update()
    {
        if (player.InputDirection != Vector3.zero)
        {
            if (player.CheckWall(player.InputDirection, out _))
            {
                player.RotateInstantly(player.InputDirection);
                player.ResetInput();
                return;
            }

            if (player.CheckBounce(player.InputDirection, out _))
            {
                player.FSMMachine.ChangeState(new KnockbackState(player, -player.InputDirection));
                return;
            }

            player.FSMMachine.ChangeState(new MoveState(player));
        }
    }

    public void Exit() { }
}
