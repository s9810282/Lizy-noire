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

    UltAttack,
}

public enum Itemtype
{
    UltSmallItem,
    UltMediumItem,
    UltLargeItem
}

[System.Serializable]
public class ItemEntry
{
    public Itemtype type;
    public GameObject item;
}

[System.Serializable]
public class ItemRequest : ScriptableObject
{
    public Itemtype type;
    public Transform parent;

    public Vector3 offset;

}



public class EventManager : MonoBehaviour
{

    public List<EffectEntry> effectList;

    private Dictionary<EffectType, EffectData> effectDict;
    private Dictionary<string, EffectObj> currentEffects = new Dictionary<string, EffectObj>();

    public List<ItemEntry> itemList;
    private Dictionary<Itemtype, GameObject> itemDict;

    private void OnEnable()
    {
        effectDict = effectList.ToDictionary(e => e.type, e => e.data);
        itemDict = itemList.ToDictionary(e => e.type, e => e.item);

        EventBus.Subscribe<EffectRequest>(OnEffectRequested);
        EventBus.Subscribe<DeathEvent>(OnDeath);
        EventBus.Subscribe<UltEvent>(OnUlt);
        EventBus.Subscribe<string>(RemoveEffectRequested);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EffectRequest>(OnEffectRequested);
        EventBus.Unsubscribe<DeathEvent>(OnDeath);
        EventBus.Unsubscribe<UltEvent>(OnUlt);
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
        else if (currentEffects.ContainsKey(req.effectCode))
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

    #region Item

    private void OnItemRequested(ItemRequest req)
    {
        if (!itemDict.TryGetValue(req.type, out var data))
        {
            Debug.LogWarning($"EffectType '{req.type}' not registered.");
            return;
        }
        
        GameObject fx = Instantiate(itemDict[req.type]);
        fx.transform.localPosition = itemDict[req.type].transform.localPosition + req.offset;
    }

    #endregion

    private void OnUlt(UltEvent e)
    {
        foreach (Vector3 line in e.range)
        {
            OnEffectRequested(new EffectRequest
            {
                effectCode = "PlayerUlt" + line,
                type = EffectType.Ult,
                offset = line,
                parent = transform,
                duration = 1f,
            });

            StartCoroutine(WaitEventDestroy("PlayerUlt" + line, 1f));
        }
    }


    IEnumerator WaitEventDestroy(string code, float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveEffectRequested(code);
    }


    #region Death

    private void OnDeath(DeathEvent e)
    {
        OnEffectRequested(e.req);
        OnItemRequested(new ItemRequest
        {
            type = Itemtype.UltSmallItem,
            parent = null,
            offset = e.target.transform.position,
        });
        StartCoroutine(WaitEventDestroy(e));
    }
    IEnumerator WaitEventDestroy(DeathEvent e)
    {
        yield return new WaitForSeconds(e.duration);
        RemoveEffectRequested(e.req.effectCode);
        Destroy(e.target);
    }

    #endregion
}
