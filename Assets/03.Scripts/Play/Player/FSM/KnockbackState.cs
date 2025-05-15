using Unity.VisualScripting;
using UnityEngine;

public class KnockbackState : IState
{
    private PlayerController player;
    private Vector3 direction;
    private Monster target;

    public KnockbackState(PlayerController player, Vector3 direction, Monster target)
    {
        this.player = player;
        this.direction = direction;
        this.target = target;
    }

    public void Enter()
    {
        if (player.CheckEffect(EStatusEffect.Invincible) || player.IsKnockedBack)
        {
            player.IsKnockedBack = false;
            player.FSMMachine.ChangeState(new IdleState(player));
            return; 
        }

        player.PlayerState = EPlayerState.KnockBack;
        player.StartKnockback(direction, target);
    }

    public void Update() 
    { 
            
    }


    public void Exit() 
    { 

    }
}
