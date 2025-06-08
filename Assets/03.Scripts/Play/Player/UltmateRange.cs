using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UltLine
{
    public List<Vector2Int> cells = new();
}

[CreateAssetMenu(fileName = "UltmateRange", menuName = "Scriptable Objects/UltmateRange")]
[System.Serializable]
public class UltmateRange : ScriptableObject
{
    public List<UltLine> range = new();
}
