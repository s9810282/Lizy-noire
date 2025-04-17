using UnityEngine;

public class Monster : MonoBehaviour, IDamageAble
{
    //추후 Spawner랑 MapLoader랑 만들어서 Spawner가 Canvas를 들고있게해서 MapLoad 시에 넣어줄 수 있게 하기.


    [SerializeField] MonsterData data;
    [SerializeField] HpBar hpBar;

    [SerializeField] float currentHP;

    public void TakeDamage(float damage)
    {
        hpBar.gameObject.SetActive(true);

        currentHP -= damage;
        hpBar.SetHP(currentHP, data.maxHp);


        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHP = data.maxHp;
        hpBar.SetHP(currentHP, data.maxHp);
        hpBar.gameObject.SetActive(false);
    }
}
