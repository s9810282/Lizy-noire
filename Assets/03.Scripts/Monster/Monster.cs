using UnityEngine;

public class Monster : MonoBehaviour, IDamageAble
{
    //���� Spawner�� MapLoader�� ���� Spawner�� Canvas�� ����ְ��ؼ� MapLoad �ÿ� �־��� �� �ְ� �ϱ�.


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
