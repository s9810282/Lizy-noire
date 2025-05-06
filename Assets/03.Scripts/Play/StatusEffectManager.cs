using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class StatusEffectManager
{
    [SerializeField] Dictionary<EStatusEffect, StatusEffect> effectList = new Dictionary<EStatusEffect, StatusEffect>();

    public void AddEffect(StatusEffect effect)
    {
        EStatusEffect eStatus = effect.eStatusEffect;

        if (effectList.ContainsKey(eStatus))
        {
            RemoveEffect(eStatus);
        }

        effect.ApplyEffect();
        effectList.Add(eStatus, effect);
    }
    
    public void Update()
    {
        if (effectList.Count == 0) return;

        var keys = new List<EStatusEffect>(effectList.Keys); // 복사본 키 리스트로 반복

        foreach (var key in keys)
        {
            var effect = effectList[key];
            effect.UpdateEffect();
            effect.Updateduration(Time.deltaTime);

            if (effect.Duration <= 0)
            {
                effect.RemoveEffect();
                effectList.Remove(key);
            }
        }
    }

    public void RemoveEffect(EStatusEffect type)
    {
        if (!effectList.ContainsKey(type)) return;

        effectList[type].RemoveEffect();
        effectList.Remove(type);
    }

    public bool HasEffect(EStatusEffect type)
    {
        return effectList.ContainsKey(type);
    }

    public StatusEffect GetEffect(EStatusEffect type)
    {
        return effectList.TryGetValue(type, out var effect) ? effect : null;
    }


    public void CheckBoostorEXhaustion(StatusEffect effect)
    {
        if(effect.eStatusEffect == EStatusEffect.Exhaustion)
        {
            if (effectList.ContainsKey(EStatusEffect.SpeedUp))
            {
                RemoveEffect(EStatusEffect.SpeedUp);
            }

        }
        //부스트와 탈진 상태는 공존 불가. 그에 따른 처리
    }
}
