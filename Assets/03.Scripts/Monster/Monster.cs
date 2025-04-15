using UnityEngine;

public class Monster : MonoBehaviour, IDamageAble
{
    [SerializeField] MonsterData data;
    [SerializeField] HpBar hpBar;

    [SerializeField] float currentHP;

    public void TakeDamage(float damage)
    {
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
    }
}
