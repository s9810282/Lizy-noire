using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExhaustionBuff : StatusEffect
{
    float speedValue;
    float damageValue;
    

    public ExhaustionBuff
        (string name, float duration, IEffectTarget target, EStatusEffect eStatusEffect, float value, float damageValue2) 
        : base(name, duration, target, eStatusEffect)
    {
        this.speedValue = value;
        this.damageValue = damageValue2;
    }

    public override void ApplyEffect()
    {
        Target.SetDamaAble(true);
        Target.ModifyMoveSpeed(-speedValue);
    }

    public override void UpdateEffect()
    {
        
    }

    public override void RemoveEffect()
    {
        Target.SetDamaAble(false);
        Target.ModifyMoveSpeed(speedValue);
    }
}
