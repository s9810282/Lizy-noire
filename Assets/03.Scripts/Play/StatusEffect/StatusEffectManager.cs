using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;




[System.Serializable]
public class StatusEffectManager
{
    [SerializeField] Dictionary<EStatusEffect, StatusEffect> effectList = new Dictionary<EStatusEffect, StatusEffect>();
    [SerializeField] List<EStatusEffect> InsfectorList = new List<EStatusEffect>();

    public void AddEffect(StatusEffect effect)
    {
        if (effect == null) return;

        EStatusEffect eStatus = effect.eStatusEffect;
        

        if (effectList.ContainsKey(eStatus))
        {
            RemoveEffect(eStatus);
            InsfectorList.Remove(eStatus);
        }

        CheckBoostorExhaustion(effect.eStatusEffect);
        CheckInvincibble();

        effect.ApplyEffect();
        effectList.Add(eStatus, effect);
        InsfectorList.Add(eStatus);
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
                InsfectorList.Remove(key);
                effectList.Remove(key);
            }
        }
    }

    public void RemoveEffect(EStatusEffect type)
    {
        if (!effectList.ContainsKey(type)) return;

        effectList[type].RemoveEffect();
        InsfectorList.Remove(type);
        effectList.Remove(type);
    }

    public StatusEffect GetEffect(EStatusEffect type)
    {
        return effectList.TryGetValue(type, out var effect) ? effect : null;
    }


    public void CheckBoostorExhaustion(EStatusEffect effect)
    {
        if(effect == EStatusEffect.Exhaustion)
        {
            if (effectList.ContainsKey(EStatusEffect.SpeedUp))
            {
                RemoveEffect(EStatusEffect.SpeedUp);
            }
        }
        //부스트와 탈진 상태는 공존 불가. 그에 따른 처리
    }
    public void CheckInvincibble()
    {
        if (effectList.ContainsKey(EStatusEffect.Exhaustion))
        {
            RemoveEffect(EStatusEffect.Exhaustion);
        }
    }


    public bool CheckStatus(EStatusEffect eStatus)
    {
        return effectList.ContainsKey(eStatus);
    } 
}
