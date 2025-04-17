using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class StatusEffect
{
    private string buffName;
    private float duration;

    protected GameObject target;

    public float Duration { get => duration; set => duration = value; }
    public string BuffName { get => buffName; set => buffName = value; }

    public StatusEffect(string name, float duration, GameObject target = null)
    {
        this.buffName = name;
        this.duration = duration;
        this.target = target;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
    public abstract void ApplyEffect();
    public abstract void RemoveEffect();

    public virtual void Updateduration(float time)
    {
        duration -= time;
    }
}
