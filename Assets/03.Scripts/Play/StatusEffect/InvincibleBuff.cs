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
        EventBus.Publish(new EffectRequest
        {
            effectCode = "InvinsiblePlayer",
            type = EffectType.Invinsible,
            parent = Target.GetTarget().gameObject.transform,
            offset = new Vector3(0f, 0f, 0f),
        });
    }

    public override void UpdateEffect()
    {

    }

    public override void RemoveEffect()
    {
        EventBus.Publish("InvinsiblePlayer");
    }
}