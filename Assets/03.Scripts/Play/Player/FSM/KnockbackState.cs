using UnityEngine;

public class KnockbackState : IState
{
    private PlayerController player;
    private Vector3 direction;

    public KnockbackState(PlayerController player, Vector3 direction)
    {
        this.player = player;
        this.direction = direction;
    }

    public void Enter()
    {
        player.StartKnockback(direction);
    }

    public void Update() 
    { 
        
    }

    public void Exit() { }
}
