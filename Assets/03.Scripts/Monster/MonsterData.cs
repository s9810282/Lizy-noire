using UnityEngine;

public enum MonsterPattern
{
    Roam,
    Patrol,

}



[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string name;
    public string damage;

    public string maxHp;
}
