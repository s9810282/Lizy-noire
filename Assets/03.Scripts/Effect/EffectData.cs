using UnityEngine;


[CreateAssetMenu(fileName = "EffectData", menuName = "Scriptable Objects/EffectData")]
public class EffectData : ScriptableObject
{
    public string effectName;
    public GameObject effectPrefab;
    public Vector3 localPositionOffset;
    public Vector3 localRotation;
    public Vector3 localScale = Vector3.one;
    public float duration = 1.5f;
}
