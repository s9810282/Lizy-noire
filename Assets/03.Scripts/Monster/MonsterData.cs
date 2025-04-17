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
    public float damage;

    public float maxHp;

    public int shieldDir;
    public int monsterDrop;
    public int bounceDistance;
}
