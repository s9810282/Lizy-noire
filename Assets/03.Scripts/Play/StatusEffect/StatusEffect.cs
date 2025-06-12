using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectTarget
{
    void ModifyMoveSpeed(float factor);
    void TakeTickDamage(float value);
    void SetDamageValue(float value);
    Transform GetTarget();
}

public enum EStatusEffect
{ 
    Exhaustion,
    SpeedUp,
    Invincible,
}



[System.Serializable]
public abstract class StatusEffect
{
    private string buffName;
    private float duration;
    private IEffectTarget target;
    private EStatusEffect statusEffect;
    protected bool isEffect;

    public float Duration { get => duration; set => duration = value; }
    public string BuffName { get => buffName; set => buffName = value; }
    public IEffectTarget Target { get => target; set => target = value; }
    public EStatusEffect eStatusEffect { get => statusEffect; set => statusEffect = value; }
    

    public StatusEffect(string name, float duration, IEffectTarget target, EStatusEffect eStatusEffect, bool isEffect = true)
    {
        this.buffName = name;
        this.duration = duration;
        this.target = target;  
        this.statusEffect = eStatusEffect;
        this.isEffect = isEffect;
    }


    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
    public virtual void UpdateEffect()
    {

    }
    public virtual void Updateduration(float time)
    {
        duration -= time;
    }
}
