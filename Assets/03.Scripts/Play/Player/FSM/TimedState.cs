using UnityEngine;

public abstract class TimedState : IState
{
    protected float duration;
    protected float timer;
    protected PlayerController player;

    public TimedState(PlayerController player, float duration)
    {
        this.player = player;
        this.duration = duration;
    }

    public virtual void Enter()
    {
        timer = duration;
    }

    public virtual void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            ExitToDefaultState();
    }

    public abstract void ExitToDefaultState();
    public abstract void Exit();
}
