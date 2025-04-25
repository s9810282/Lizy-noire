using UnityEngine;

public class MoveState : IState
{
    private PlayerController player;

    private bool isAlready = false;
    private Vector3 targetforward = Vector3.zero;

    public MoveState(PlayerController player, bool isAlready = false)
    {
        this.player = player;
        this.isAlready = isAlready;
    }

    public void Enter()
    {
        player.PlayerState = EPlayerState.Move;

        if (CheckWall()) return;
        CheckBounce();

        if (!isAlready)
        {
            Vector3 nextPos = player.TargetPosition + player.InputDirection;
            player.SetTargetPosition(nextPos);
            targetforward = player.GetPlayerToTargetFoward();
        }
    }

    public void Update()
    {
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


    public bool CheckBounce()
    {
        if (player.CheckBounce(player.InputDirection, out RaycastHit bounceHit))
        {
            Monster target = bounceHit.collider.GetComponent<Monster>();

            if (target != null)
            {
                player.FSMMachine.ChangeState(new KnockbackState(player, -player.InputDirection, target));
                return true;
            }
        }

        return false;
    }
    public bool CheckWall()
    {
        if (player.CheckWall(player.InputDirection, out RaycastHit wallHit))
        {
            player.SetTargetPosition(player.transform.position);
            player.RotateInstantly(player.InputDirection);
            return true;
        }

        return false;
    }
}
