using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class ExhaustionBuff : StatusEffect
{
    float speedValue;
    float damageValue;

    bool isWall = false;
    string code = "";

    public ExhaustionBuff
        (string name, float duration, IEffectTarget target, EStatusEffect eStatusEffect, float value, float damageValue2, bool isWall = false, bool isEffect = true) 
        : base(name, duration, target, eStatusEffect, isEffect)
    {
        this.speedValue = value;
        this.damageValue = damageValue2;
        this.isWall = isWall;
    }

    public override void ApplyEffect()
    {
        Target.SetDamageValue(damageValue);
        Target.ModifyMoveSpeed(-speedValue);
        
        EffectType e;

        if (isEffect)
        {
            code = "PlayerExhaust";

            if (isWall) e = EffectType.LongStun;
            else e = EffectType.Stun;
        }
        else
        {
            code = "PlayerSpeedDown";
            e = EffectType.SpeedDown;
        }

        EventBus.Publish(new EffectRequest
        {
            effectCode = code,
            type = e,
            parent = Target.GetTarget(),
            duration = Duration
        });
    }

    public override void UpdateEffect()
    {
        
    }

    public override void RemoveEffect()
    {
        Target.SetDamageValue(0);
        Target.ModifyMoveSpeed(speedValue);

        EventBus.Publish(code);
    }
}
