using UnityEngine;

public enum MonsterPattern
{
    Roam,
    Patrol,

}

[System.Serializable]
public struct MonsterValue
{
    public int shieldDir;
    public int monsterDrop;
    public int bounceDistance;
}


[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string name;

    public float damage;
    public float maxHp;
    public float speed;

    public MonsterValue monsterValue;
}
