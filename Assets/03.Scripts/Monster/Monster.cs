using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using static EventManager;

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
    //추후 Spawner랑 MapLoader랑 만들어서 Spawner가 Canvas를 들고있게해서 MapLoad 시에 넣어줄 수 있게 하기.
    [SerializeField] MonsterData data;
    [SerializeField] HpBar hpBar;
    [SerializeField] Animator animator;
    [SerializeField] BehaviorGraphAgent bt;
    [SerializeField] BoxCollider col;

    [SerializeField] float currentHP;
    [SerializeField] float currentSpeed;
    [SerializeField] bool isGroggy = false;
    [SerializeField] bool isMove = false;
    [SerializeField] bool isDead = false;
    [SerializeField] List<RelativeDirection> shieldDir = new List<RelativeDirection>();

    [SerializeField] PathNode targetNode;
    [SerializeField] PathNode nextTargetNode;
    [SerializeField] List<PathNode> pathNodes = new List<PathNode>();

    [Header("Layers")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask PlayerLayer;

    int currentMovePathCount = 0;

    public MonsterData Data { get => data; set => data = value; }
    public PathNode TargetNode { get => targetNode; set => targetNode = value; }
    public List<PathNode> PathNodes { get => pathNodes; set => pathNodes = value; }
    public HpBar HpBar { get => hpBar; set => hpBar = value; }

    public void TakeDamage(float damage, EAttakcType e)
    {
        HitedPlayer();

        currentHP -= damage;
        hpBar.SetHP(currentHP, data.maxHp);
        hpBar.HideHpBar(true);

        if (currentHP <= 0)
        {
            isDead = true;

            bt.enabled = false;
            col.enabled = false;

            SetAnimTrigger("Die");
            hpBar.HideHpBar(false);

            EventBus.Publish(new DeathEvent
            {
                target = this.gameObject,
                duration = 2f,
                req = new EffectRequest
                {
                    effectCode = "MonsterDeath" + name,
                    type = EffectType.DeathSpark,
                    parent = transform,
                    offset = new Vector3(0f, 0.5f, 0f),
                }
            });
        }
        else
        {
            if (e == EAttakcType.Blow)
            {
                EventBus.Publish(new EffectRequest
                {
                    effectCode = "MonsterBlueSpark" + name,
                    type = EffectType.BlowSpark,
                    offset = transform.position,
                    parent = null,
                });

                StartCoroutine(RemoveEffect("MonsterBlueSpark" + name, 0.5f));
            }
            else if (e == EAttakcType.Slash)
            {
                EventBus.Publish(new EffectRequest
                {
                    effectCode = "MonsterRedSpark" + name,
                    type = EffectType.SlashSpark,
                    offset = transform.position,
                    parent = null,
                });

                StartCoroutine(RemoveEffect("MonsterRedSpark" + name, 0.5f));


                EventBus.Publish(new EffectRequest
                {
                    effectCode = "PlayerSlashHit",
                    type = EffectType.SlashHit,
                    offset = transform.position,
                    parent = null,
                });

                StartCoroutine(RemoveEffect("PlayerSlashHit", 0.5f));
            }
        }
    }

    public void ShowHitSpark(EAttakcType e)
    {

    }

    void Start()
    {
        currentHP = data.maxHp;
        currentSpeed = data.speed;

        hpBar.SetTarget(transform);
        hpBar.SetHP(currentHP, data.maxHp);
        hpBar.HideHpBar(false);

        nextTargetNode = new PathNode();
        nextTargetNode.worldPosition = transform.position;

        isGroggy = false;
        isDead = false;
        isMove = false;

        AddShield();
    }


    Coroutine attackCoroitine;
    public void AttackPlayer()
    {
        if (isGroggy)
            StopCoroutine(attackCoroitine);

        Debug.Log("Attack Player");
        isGroggy = true;
        SetAnimTrigger("Hit");

        attackCoroitine = StartCoroutine(WaitAttackGroggy());
    }


    Coroutine hitCoroitine;
    public void HitedPlayer()
    {
        if (isGroggy)
            StopCoroutine(hitCoroitine);

        Debug.Log("Hit Player");
        isGroggy = true;
        SetAnimTrigger("Down");
        EventBus.Publish(new EffectRequest
        {
            effectCode = "MonsterStun" + name,
            type = EffectType.Stun,
            parent = transform,
            offset = new Vector3(0f, 0.5f, 0f),
            duration = data.groggyDuration
        });


        hitCoroitine = StartCoroutine(WaitGroggy());
    }

    public void CheckGroggyAnim()
    {
        if (!isGroggy)
            SetAnimTrigger("Walk");
    }

    IEnumerator WaitGroggy()
    {
        yield return new WaitForSeconds(data.groggyDuration * 0.8f);
        SetAnimTrigger("Up");
        EventBus.Publish("MonsterStun" + name);
        yield return new WaitForSeconds(data.groggyDuration * 0.2f);
        isGroggy = false;

        if(isMove)
            SetAnimTrigger("Walk");
            
    }

    IEnumerator WaitAttackGroggy()
    {
        yield return new WaitForSeconds(data.groggyDuration);
        isGroggy = false;

        if (isMove)
            SetAnimTrigger("Walk");
        else
            SetAnimTrigger("Idle");
    }




    public Node.Status Move()
    {
        if (isGroggy) return Node.Status.Running;

        if (CheckBound())
        {
            isMove = false;
            return Node.Status.Success;
        }

        isMove = true;

        RotateTowardsDirection();
        transform.position = Vector3.MoveTowards(transform.position,
            nextTargetNode.worldPosition, Time.deltaTime * currentSpeed);

        if (Vector3.Distance(transform.position, nextTargetNode.worldPosition) < 0.01f)
        {
            transform.position = nextTargetNode.worldPosition;
            if (transform.position == targetNode.worldPosition)
            {
                isMove = false;
                SetAnimTrigger("Idle");
                return Node.Status.Success;
            }

            UpdateTarget();
        }

        return Node.Status.Running;
    }


    public void UpdateTarget()
    {
        currentMovePathCount++;
        nextTargetNode = pathNodes[currentMovePathCount];
    }
    public void RotateTowardsDirection()
    {
        Vector3 targetDir = (nextTargetNode.worldPosition - transform.position).normalized;
        targetDir.y = 0;

        if (targetDir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 100f);
    }
    public void RotateInstantly(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        float yRotation = DirToYRotation(dir);
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = targetRotation;
    }
    private float DirToYRotation(Vector3 dir)
    {
        if (dir == Vector3.forward) return 0f;
        if (dir == Vector3.right) return 90f;
        if (dir == Vector3.back) return 180f;
        if (dir == Vector3.left) return 270f;
        return 0f;
    }

    public void FindPath()
    {
        PathNode current = PathFinder.Instance.pathFinding.grid.GetGridObject3D(nextTargetNode.worldPosition);
        pathNodes = PathFinder.Instance.pathFinding.FindPath((int)current.worldPosition.x, (int)current.worldPosition.z,
            (int)targetNode.worldPosition.x, (int)targetNode.worldPosition.z);

        currentMovePathCount = -1;

        if (pathNodes != null)
        {
            for (int i = 0; i < pathNodes.Count - 1; i++)
            {
                //Debug.Log(pathNodes[i].worldPosition.x + ", " + pathNodes[i].worldPosition.z);

                Debug.DrawLine(new Vector3(pathNodes[i].worldPosition.x, 2, pathNodes[i].worldPosition.z) * 1f + Vector3.one * 0f,
                    new Vector3(pathNodes[i + 1].worldPosition.x, 2, pathNodes[i + 1].worldPosition.z) * 1f + Vector3.one * 0f, Color.red, 100f);
            }
        }
    }
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
    public void SetAnimBool(string name, bool val)
    {
        animator.SetBool(name, val);
    }
    public void SetAnimTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    IEnumerator RemoveEffect(string effectcode, float duration)
    {
        yield return new WaitForSeconds(duration);
        EventBus.Publish(effectcode);
    }

    #region Physcs

    public bool RaycaseWall(Vector3 dir, out RaycastHit hit, float distance = 0.8f)
    {
        return Physics.Raycast(transform.position, dir, out hit, distance, wallLayer);
    }
    public bool RaycasePlayer(Vector3 dir, out RaycastHit hit, float distance = 0.8f)
    {
        Vector3 pos = transform.position;
        return Physics.Raycast(pos, dir, out hit, distance, PlayerLayer);
    }
    public bool RaycaseBouncedPlayer(Vector3 dir, out RaycastHit hit, float distance = 0.8f)
    {
        int combinedMask = wallLayer | PlayerLayer;

        Vector3 pos = transform.position;
        Physics.Raycast(pos, dir, out hit, distance, combinedMask);

        if (hit.collider != null)
        {
            int hitLayerBit = 1 << hit.collider.gameObject.layer;
            return (PlayerLayer.value & hitLayerBit) != 0;
        }
        else
            return false;
    }

    public bool CheckBound()
    {
        if (RaycasePlayer(transform.forward, out RaycastHit bounceHit))
        {
            PlayerController target = bounceHit.collider.GetComponent<PlayerController>();
            if (target.isInvinvible) return false;
            target.FSMMachine.ChangeState(new KnockbackState(target, transform.forward, this));
            return true;
        }

        return false;
    }


    #endregion




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
        Debug.Log(dir);

        return shieldDir.Contains(dir);
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
