using UnityEngine;

public class IdleState : IState
{
    private PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter() 
    {
        player.PlayerState = EPlayerState.Idle;
    }

    public void Update()
    {
        if (player.CheckBounce(player.InputDirection, out RaycastHit bounceHit))
        {
            Monster target = bounceHit.collider.GetComponent<Monster>();

            if (target != null)
            {
                player.FSMMachine.ChangeState(new KnockbackState(player, -player.InputDirection, target));
                return;
            }            
        }

        if (player.InputDirection != Vector3.zero)
        {
            if (player.CheckWall(player.InputDirection, out _))
            {
                player.RotateInstantly(player.InputDirection);
                player.ResetInput();
                return;
            }

            player.FSMMachine.ChangeState(new MoveState(player));
        }
    }

    public void Exit() { }
}
