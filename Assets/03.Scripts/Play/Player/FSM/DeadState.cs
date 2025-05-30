using UnityEngine;

public class DeadState : IState
{
    private PlayerController player;

    public DeadState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.PlayerState = EPlayerState.Dead;

        EventBus.Publish(new EffectRequest
        {
            effectCode = "PlayerDeath",
            type = EffectType.DeathSpark,
            offset = player.gameObject.transform.position,
            parent = null,
        });
    }

    public void Update()
    {
        
    }

    public void Exit() 
    { 
        EventBus.Publish("PlayerDeath"); 
    }
}
