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
        if (CheckWall()) return;
        if (CheckBounce()) return;

        player.PlayerState = EPlayerState.Move;
        player.AnimSetBool("isMove", true);
        player.AnimSetInt("KnockBack", 0);

        if (!isAlready)
        {
            Vector3 nextPos = player.TargetPosition + player.InputDirection;
            player.SetTargetPosition(nextPos);
            targetforward = player.GetPlayerToTargetFoward();
        }
    }

    public void Update()
    {
        player.RecoveryBoost();

        player.RotateTowardsDirection(targetforward);
        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            player.TargetPosition,
            player.CurrentMoveSpeed * Time.deltaTime);

        CheckBounce();

        if (Vector3.Distance(player.transform.position, player.TargetPosition) < 0.01f)
        {
            player.RotateInstantly(targetforward);
            player.transform.position = player.TargetPosition;

            if (player.InputDirection != Vector3.zero)
            {
                Enter();
                //player.FSMMachine.ChangeState(new MoveState(player));
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
        if (player.RaycaseBounce(player.InputDirection, out RaycastHit bounceHit))
        {
            if (bounceHit.collider.TryGetComponent<IValueItem>(out IValueItem item))
            {
                player.UpdateUltValue((int)item.Execute());
            }
            else
            {
                Monster target = bounceHit.collider.GetComponent<Monster>();

                if (target != null)
                {
                    player.FSMMachine.ChangeState(new KnockbackState(player, -player.InputDirection, target));
                    return true;
                }
            }
        }

        return false;
    }
    public bool CheckWall()
    {
        if (player.RaycaseWall(player.InputDirection, out RaycastHit wallHit))
        {
            player.SetTargetPosition(player.transform.position);
            player.RotateInstantly(player.InputDirection);
            return true;
        }

        return false;
    }
}
