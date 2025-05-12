using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public enum RelativeDirection
{
    Front,
    Back,
    Left,
    Right
}

public enum EMonsterPattern
{
    Roam,

}

public class Monster : MonoBehaviour, IDamageAble
{
    //���� Spawner�� MapLoader�� ���� Spawner�� Canvas�� ����ְ��ؼ� MapLoad �ÿ� �־��� �� �ְ� �ϱ�.
    [SerializeField] MonsterData data;
    [SerializeField] HpBar hpBar;

    [SerializeField] float currentHP;
    [SerializeField] List<RelativeDirection> shieldDir = new List<RelativeDirection>();

    [SerializeField] PathNode targetNode;
    [SerializeField] PathNode nextTargetNode;
    [SerializeField] List<PathNode> pathNodes = new List<PathNode>();

    int currentMovePathCount = 0;

    public MonsterData Data { get => data; set => data = value; }
    public PathNode TargetNode { get => targetNode; set => targetNode = value; }
    public List<PathNode> PathNodes { get => pathNodes; set => pathNodes = value; }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        hpBar.SetHP(currentHP, data.maxHp);
        hpBar.HideHpBar(true);

        if (currentHP <= 0)
        {
            hpBar.HideHpBar(false);
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHP = data.maxHp;
        hpBar.SetHP(currentHP, data.maxHp);
        hpBar.HideHpBar(false);

        AddShield();
    }




    public Node.Status Move()
    {
        RotateTowardsDirection();
        transform.position = Vector3.MoveTowards(transform.position,
            nextTargetNode.worldPosition,  Time.deltaTime * data.speed);

        if(Vector3.Distance(transform.position, nextTargetNode.worldPosition) < 0.01f)
        {
            transform.position = nextTargetNode.worldPosition;
            if (transform.position == targetNode.worldPosition)
                return Node.Status.Success;

            UpdateTarget();
        }

        return Node.Status.Running;
    }

    public void UpdateTarget()
    {
        pathNodes.Remove(nextTargetNode);
        nextTargetNode = pathNodes[0];
    }

    public void FindPath()
    {
        PathNode current = PathFinder.Instance.pathFinding.grid.GetGridObject3D(transform.position);
        pathNodes = PathFinder.Instance.pathFinding.FindPath((int)current.worldPosition.x, (int)current.worldPosition.z,
            (int)targetNode.worldPosition.x, (int)targetNode.worldPosition.z);

        Debug.Log(current.worldPosition);
        Debug.Log(targetNode.worldPosition);

        if (pathNodes != null)
        {
            for (int i = 0; i < pathNodes.Count - 1; i++)
            {
                Debug.Log(pathNodes[i].worldPosition.x + ", " + pathNodes[i].worldPosition.z);

                Debug.DrawLine(new Vector3(pathNodes[i].worldPosition.x, 2, pathNodes[i].worldPosition.z) * 1f + Vector3.one * 0f,
                    new Vector3(pathNodes[i + 1].worldPosition.x, 2, pathNodes[i + 1].worldPosition.z) * 1f + Vector3.one * 0f, Color.red, 100f);
            }
        }

        currentMovePathCount = 0;
        nextTargetNode = pathNodes[0];
    }

    public void RotateTowardsDirection()
    {
        Vector3 targetDir = (nextTargetNode.worldPosition - transform.position).normalized;
        targetDir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 100f);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }
    private float DirToYRotation(Vector3 dir)
    {
        if (dir == Vector3.forward) return 0f;
        if (dir == Vector3.right) return 90f;
        if (dir == Vector3.back) return 180f;
        if (dir == Vector3.left) return 270f;
        return 0f;
    }

    #region Shield

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

    #endregion
}
