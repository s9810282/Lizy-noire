using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum EffectType
{
    Blow,
    Slash,
    SlashHit,
    Ult,

    Stun,
    DamageAble,
    SpeedUp,
}



public class EffectManager  : MonoBehaviour
{
    [System.Serializable]
    public class EffectEntry
    {
        public EffectType type;
        public EffectData data;
    }
    [System.Serializable]
    public class EffectObj
    {
        public EffectType type;
        public GameObject obj;

        public EffectObj(EffectType type, GameObject data)
        {
            this.type = type;
            this.obj = data;
        }
    }

    public List<EffectEntry> effectList;

    private Dictionary<EffectType, EffectData> effectDict;
    private Dictionary<string, EffectObj> currentEffects = new Dictionary<string, EffectObj>();

    private void OnEnable()
    {
        effectDict = effectList.ToDictionary(e => e.type, e => e.data);
        EventBus.Subscribe<EffectRequest>(OnEffectRequested);
        EventBus.Subscribe<string>(RemoveEffectRequested);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EffectRequest>(OnEffectRequested);
        EventBus.Unsubscribe<string>(RemoveEffectRequested);
    }

    private void OnEffectRequested(EffectRequest req)
    {
        if (!effectDict.TryGetValue(req.type, out var data))
        {
            Debug.LogWarning($"EffectType '{req.type}' not registered.");
            return;
        }

        GameObject fx = Instantiate(data.effectPrefab, req.parent);
        fx.transform.localPosition = data.localPositionOffset + req.offset;
        fx.transform.localEulerAngles = data.localRotation;
        fx.transform.localScale = data.localScale;

        currentEffects.Add(req.effectCode, new EffectObj(req.type, fx));
    }

    private void RemoveEffectRequested(string code)
    {
        if (!currentEffects.ContainsKey(code)) return;

        Destroy(currentEffects[code].obj);
        currentEffects.Remove(code);
    }
}
