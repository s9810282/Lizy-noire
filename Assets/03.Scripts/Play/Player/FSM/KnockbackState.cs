using Unity.VisualScripting;
using UnityEngine;

public class KnockbackState : IState
{
    private PlayerController player;
    private Vector3 direction;
    private Monster target;

    float boostDuration;
    bool isBoost;

    public KnockbackState(PlayerController player, Vector3 direction, Monster target, float boostDuration = 0f, bool isBoost = false)
    {
        this.player = player;
        this.direction = direction;
        this.target = target;
        this.boostDuration = boostDuration;
        this.isBoost = isBoost;
    }

    public void Enter()
    {
        if (player.CheckEffect(EStatusEffect.Invincible) || player.IsKnockedBack) //RaycaseBounce에서 한번 검사해서 없어도 되려나
        {
            player.IsKnockedBack = false;
            player.FSMMachine.ChangeState(new IdleState(player));
            return; 
        }

        player.PlayerState = EPlayerState.KnockBack;
        player.StartKnockback(direction, target, isBoost:this.isBoost, boostDuration:this.boostDuration);
    }

    public void Update() 
    { 
            
    }


    public void Exit() 
    { 

    }
}
