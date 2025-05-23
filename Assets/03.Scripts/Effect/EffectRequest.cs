using UnityEngine;

[CreateAssetMenu(fileName = "EffectRequest", menuName = "Scriptable Objects/EffectRequest")]
public class EffectRequest : ScriptableObject
{
    public string effectCode;
    public EffectType type;
    public Transform parent;

    public Vector3 offset;
    public float duration;
}
