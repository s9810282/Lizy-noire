using UnityEngine;


public enum EffectType
{
    Blow,
    Slash,
    Ult,

    Stun,
    Invinsible,

}



public class EffectManager : MonoBehaviour
{
    [SerializeField] SerializableDictionary<EffectType, Effect> effectList;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
