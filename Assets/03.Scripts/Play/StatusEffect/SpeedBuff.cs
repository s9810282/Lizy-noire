using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeedBuff : StatusEffect
{
    float speedValue;

    public SpeedBuff
        (string name, float duration, IEffectTarget target, EStatusEffect eStatusEffect, float value) 
        : base(name, duration, target, eStatusEffect)
    {
        this.speedValue = value;
    }

    public override void ApplyEffect()
    {
        Target.ModifyMoveSpeed(speedValue);
        EventBus.Publish(new EffectRequest
        {
            effectCode = "PlayerSpeed",
            type = EffectType.SpeedUp,
            parent = Target.GetTarget(),
            duration = Duration
        });
    }

    public override void UpdateEffect()
    {
        
    }

    public override void RemoveEffect()
    {
        Target.ModifyMoveSpeed(-speedValue);
        EventBus.Publish("PlayerSpeed");
    }
}
