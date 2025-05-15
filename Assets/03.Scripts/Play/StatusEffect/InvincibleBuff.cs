using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InvincibleBuff : StatusEffect
{


    public InvincibleBuff
        (string name, float duration, IEffectTarget target, EStatusEffect eStatusEffect)
        : base(name, duration, target, eStatusEffect)
    {

    }

    public override void ApplyEffect()
    {

    }

    public override void UpdateEffect()
    {

    }

    public override void RemoveEffect()
    {
        
    }
}