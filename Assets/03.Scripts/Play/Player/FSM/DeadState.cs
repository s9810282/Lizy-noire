using UnityEngine;
using static EventManager;

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

        EventBus.Publish(new DeathEvent
        {
            target = player.gameObject,
            duration = 1f,
            req = new EffectRequest
            {
                effectCode = "PlayerDeath",
                type = EffectType.DeathSpark,
                offset = player.gameObject.transform.position,
                parent = null,
            },
            itemreq = null
        });

        EventBus.Publish(new StageResultEvent { e = EStageResultType.Fail });
    }

    public void Update()
    {
        
    }

    public void Exit() 
    { 
        
    }
}
