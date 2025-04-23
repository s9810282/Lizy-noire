using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum RelativeDirection
{
    Front,
    Back,
    Left,
    Right
}


public class Monster : MonoBehaviour, IDamageAble
{
    //추후 Spawner랑 MapLoader랑 만들어서 Spawner가 Canvas를 들고있게해서 MapLoad 시에 넣어줄 수 있게 하기.
    [SerializeField] MonsterData data;
    [SerializeField] HpBar hpBar;

    [SerializeField] float currentHP;
    [SerializeField] List<RelativeDirection> shieldDir = new List<RelativeDirection>();

    public MonsterData Data { get => data; set => data = value; }


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

        AddShield();
    }



    public void AddShield()
    {
        switch (data.monsterValue.shieldDir)
        {
            case 0:
                break;
            case 1:
                shieldDir.Add(RelativeDirection.Front);
                break;
            case 2:
                shieldDir.Add(RelativeDirection.Right);
                break;
            case 3:
                shieldDir.Add(RelativeDirection.Left);
                break;
            case 4:
                shieldDir.Add(RelativeDirection.Back);
                break;
            case 5:
                shieldDir.Add(RelativeDirection.Right);
                shieldDir.Add(RelativeDirection.Left);
                break;
            case 6:
                shieldDir.Add(RelativeDirection.Front);
                shieldDir.Add(RelativeDirection.Back);
                break;
        }
    }

    public RelativeDirection GetRelativeDirection(Vector3 toPlayer)
    {
        float forwardDot = Vector3.Dot(transform.forward, toPlayer);
        float rightDot = Vector3.Dot(transform.right, toPlayer);

        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            return forwardDot > 0 ? RelativeDirection.Front : RelativeDirection.Back;
        }
        else
        {
            return rightDot > 0 ? RelativeDirection.Right : RelativeDirection.Left;
        }
    }

    public bool CheckShield(Vector3 toPlayer)
    {
        RelativeDirection dir = GetRelativeDirection(toPlayer);

        if (shieldDir.Contains(dir))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveShield(Vector3 toPlayer)
    {
        RelativeDirection dir = GetRelativeDirection(toPlayer);

        if (shieldDir.Contains(dir))
        {
            shieldDir.Remove(dir);
        }
    }
}
