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
        Target.SetDamaAble(true);
        Target.ModifyMoveSpeed(-speedValue);
       
        if(isEffect)
        {
            EffectType e;

            if (isWall) e = EffectType.LongStun;
            else e = EffectType.Stun;

            EventBus.Publish(new EffectRequest
            {
                effectCode = "PlayerExhaust",
                type = e,
                parent = Target.GetTarget(),
                duration = Duration
            });
        }
    }

    public override void UpdateEffect()
    {
        
    }

    public override void RemoveEffect()
    {
        Target.SetDamaAble(false);
        Target.ModifyMoveSpeed(speedValue);

        EventBus.Publish("PlayerExhaust");
    }
}
