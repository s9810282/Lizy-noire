using UnityEngine;

public abstract class TimedState : IState
{
    protected float duration;
    protected float curDuration;
    protected float timer;

    protected PlayerController player;

    public TimedState(PlayerController player, float duration, float curDuration = 0f)
    {
        this.player = player;
        this.duration = duration;
        this.curDuration = curDuration;

        if (curDuration == 0f)
            this.curDuration = this.duration;
    }

    public virtual void Enter()
    {
        timer = curDuration;
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
