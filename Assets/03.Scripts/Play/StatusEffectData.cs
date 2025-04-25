using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatusEffectData", menuName = "Scriptable Objects/StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    public EStatusEffect eStatusEffect;
    public string effectName;
    public float duration;

    public int valueArraySize = 1;
    public List<float> values;

}
