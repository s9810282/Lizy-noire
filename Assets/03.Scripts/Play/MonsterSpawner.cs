using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] HpBar monsterHpBar;
    [SerializeField] GameObject worldUICanvas;


    
    void Start()
    {
        
    }
    
    public void GiveHpBar(GameObject a)
    {
        HpBar hpBar = Instantiate(monsterHpBar, worldUICanvas.transform);
        hpBar.SetTarget(a.transform);

        a.GetComponent<Monster>().HpBar = hpBar;
    }
}
