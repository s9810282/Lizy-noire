using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EffectType
{
    Blow,
    Slash,
    SlashHit,
    Ult,

    BlowSpark,
    SlashSpark,
    DeathSpark,

    Stun,
    LongStun,
    Invinsible,
    SpeedUp,
    SpeedDown,
}


public class EventManager  : MonoBehaviour
{
  
    public List<EffectEntry> effectList;

    private Dictionary<EffectType, EffectData> effectDict;
    private Dictionary<string, EffectObj> currentEffects = new Dictionary<string, EffectObj>();

    private void OnEnable()
    {
        effectDict = effectList.ToDictionary(e => e.type, e => e.data);

        EventBus.Subscribe<EffectRequest>(OnEffectRequested);
        EventBus.Subscribe<DeathEvent>(OnDeath);
        EventBus.Subscribe<string>(RemoveEffectRequested);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EffectRequest>(OnEffectRequested);
        EventBus.Unsubscribe<DeathEvent>(OnDeath);
        EventBus.Unsubscribe<string>(RemoveEffectRequested);
    }


    #region Effect 


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


    private void OnEffectRequested(EffectRequest req)
    {
        if (!effectDict.TryGetValue(req.type, out var data))
        {
            Debug.LogWarning($"EffectType '{req.type}' not registered.");
            return;
        }
        else if(currentEffects.ContainsKey(req.effectCode))
        {
            RemoveEffectRequested(req.effectCode);
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

    #endregion


    private void OnDeath(DeathEvent e)
    {
        Debug.Log($"Á×Àº ´ë»ó: {e.target.name}");
        
        OnEffectRequested(e.req);
        StartCoroutine(WaitEventDestroy(e));
    }

    IEnumerator WaitEventDestroy(DeathEvent e)
    {
        yield return new WaitForSeconds(e.duration);
        RemoveEffectRequested(e.req.effectCode);
        Destroy(e.target);
    }


    public struct DeathEvent
    {
        public GameObject target;
        public float duration;

        public EffectRequest req;
    }



}
