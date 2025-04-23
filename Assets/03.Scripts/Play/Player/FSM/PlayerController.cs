using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Interfaces;
using System;

public class PlayerController : MonoBehaviour, IEffectTarget, IDamageAble
{
    [Header("System")]
    [SerializeField] StateMachine fsmMachine;
    [SerializeField] StatusEffectManager statusEffectManager;

    [Header("Player")]
    [SerializeField] Player player;
    [SerializeField] EAttakcType playerAttackType = EAttakcType.Blow;
    [SerializeField] EPlayerState playerState = EPlayerState.Idle;

    [Header("Player Movement")]
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float hp = 100;
    [SerializeField] private float atk = 100;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float moveSpeedModifier = 0f;
    [SerializeField] private float currentMoveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 1080f;
    [SerializeField] private float knockBackDuration = 0.25f;
    [SerializeField] private float knockBackHeight = 1f;

    [Space(15f)]

    [Header("Player KnockBack")]
    [Tooltip("언제부터 착지 가능한가 0~1")][SerializeField] private float landingTime = 0.6f;
    [Tooltip("착지 성공 최소값")][SerializeField] private float landingMin = 1.2f;
    [Tooltip("착지 성공 최대값")][SerializeField] private float landingMax = 1.8f;
    [Space(15f)]
    [SerializeField] bool isKnockedBack = false;
    [SerializeField] bool isTryLanding = false;
    [SerializeField] bool isFalling = false;
    [SerializeField] bool isDamageAble = false;

    [Header("Layers")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask bounceLayer;

    [Header("Field")]
    [SerializeField] Vector3 inputDirection = Vector3.zero;
    [SerializeField] Vector3 targetPosition;


    #region Property

    public StateMachine FSMMachine { get => fsmMachine; set => fsmMachine = value; }


    public float BaseMoveSpeed { get => baseMoveSpeed; set => baseMoveSpeed = value; }
    public float MoveSpeedModifier { get => moveSpeedModifier; set => moveSpeedModifier = value; }
    public float CurrentMoveSpeed { get => currentMoveSpeed; set => currentMoveSpeed = value; }

    public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }
    public float MaxHp { get => maxHp; set => maxHp = value; }
    public float Hp { get => hp; set => hp = value; }
    public float Atk { get => atk; set => atk = value; }

    public float KnockBackDuration { get => knockBackDuration; set => knockBackDuration = value; }
    public float KnockBackHeight { get => knockBackHeight; set => knockBackHeight = value; }


    public Vector3 InputDirection { get => inputDirection; set => inputDirection = value; }
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    
    public bool IsKnockedBack { get => isKnockedBack; set => isKnockedBack = value; }
    public bool IsDamageAble { get => isDamageAble; set => isDamageAble = value; }
    public EPlayerState PlayerState { get => playerState; set => playerState = value; }


    #endregion

    private void Awake()
    {
        fsmMachine = new StateMachine();
        statusEffectManager = new StatusEffectManager();
    }

    private void Start()
    {
        targetPosition = SnapToGrid(transform.position);
        transform.position = targetPosition;

        hp = maxHp;

        fsmMachine.ChangeState(new IdleState(this));
    }

    private void Update()
    {
        fsmMachine.Update();
        statusEffectManager.Update();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsKnockedBack) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            InputDirection = Vector3.zero;
            return;
        }

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            InputDirection = input.x > 0 ? Vector3.right : Vector3.left;
        else
            InputDirection = input.y > 0 ? Vector3.forward : Vector3.back;
    }

    

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (!IsKnockedBack) return;
        if (!isFalling) return;
        if (isTryLanding) return;
        
        Debug.Log("Space");

        isTryLanding = true;

        float yPos = transform.position.y;

        if (yPos > landingMax)  //경직
        {
            Debug.Log("Bad");
            KnockbackBeforeStatus = new ExhaustionBuff("경직", 1f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 25);
        }
        else if (yPos > landingMin) //부스트
        {
            // 부스트 활성화 시 끝나는 것도 체크해야함
            //부스트 남은 횟수, 그에 따른 시간 등 체크
            Debug.Log("Good");
            KnockbackBeforeStatus = new SpeedBuff("부스트 속도 업", 3f, this, EStatusEffect.SpeedUp, baseMoveSpeed/2f);
            KnockbackBeforeState = new BoostState(this, 3f);
        }
        else //경직
        {
            Debug.Log("Bad");
            KnockbackBeforeStatus = new ExhaustionBuff("경직", 1f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 25);
        }
    }

    public void UpdateSpeed()
    {
        currentMoveSpeed = baseMoveSpeed + moveSpeedModifier;
    }

    public void ResetInput()
    {
        InputDirection = Vector3.zero;
    }
    public void SetTargetPosition(Vector3 worldPos)
    {
        targetPosition = SnapToGrid(worldPos);
    }
    public Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            1f,
            Mathf.Round(pos.z)
        );
    }


    public void RotateTowardsDirection()
    {
        if (InputDirection == Vector3.zero) return;
        if (baseMoveSpeed <= 0) return;

        float yRotation = DirToYRotation(InputDirection);
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    public void RotateTowardsDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        float yRotation = DirToYRotation(dir);
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
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


    public bool CheckWall(Vector3 dir, out RaycastHit hit, float distance = 0.8f)
    {
        return Physics.Raycast(transform.position, dir, out hit, distance, wallLayer);
    }
    public bool CheckBounce(Vector3 dir, out RaycastHit hit)
    {
        Vector3 pos = transform.position;

        return Physics.Raycast(pos, dir, out hit, 0.8f, bounceLayer);
    }


    //Monster Data에 값에 따라서 값들 조정

    public void StartKnockback(Vector3 direction, Monster target, float distance = 2f, float height = 1f, float duration = 0.4f)
    {
        if (IsKnockedBack) return;

        if (isDamageAble)
        {
            ((IDamageAble)this).TakeDamage(0);


            //랜덤방향 이지랄
            StartCoroutine(DoParabolaKnockback(direction, 1f, height, duration));
        }
        else
        {
            Vector3 toPlayer = (transform.position - target.transform.position).normalized;
            duration = knockBackDuration * target.Data.monsterValue.bounceDistance;

            if (playerAttackType == EAttakcType.Blow)
            {
                target.RemoveShield(toPlayer);
                target.TakeDamage(atk);

                height = knockBackHeight;
                StartCoroutine(DoParabolaKnockback(direction, distance, height, duration));
            }
            else if(playerAttackType == EAttakcType.Slash)
            {
                if (!target.CheckShield(toPlayer))
                {
                    target.TakeDamage(9999);
                }
                else
                {
                    StartCoroutine(DoParabolaKnockback(direction, distance*2, 0.25f, duration));
                }
            }
        }
    }



    StatusEffect KnockbackBeforeStatus;
    IState KnockbackBeforeState;

    private IEnumerator DoParabolaKnockback(Vector3 dir, float dist, float height, float duration)
    {
        IsKnockedBack = true;
        isTryLanding = false;
        isFalling = false;

        Vector3 start = transform.position;
        Vector3 end = SnapToGrid(start + dir * dist);
        float elapsed = 0f;

        ResetInput();

        KnockbackBeforeStatus = new ExhaustionBuff("경직", 1f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 25);
        KnockbackBeforeState = new IdleState(this);


        if (playerAttackType == EAttakcType.Blow)
        {
            if (CheckWall(dir, out RaycastHit hit, dist)) //스턴
            {
                //isTryLanding = true;

                end = SnapToGrid(hit.transform.position - dir);
                KnockbackBeforeStatus = new ExhaustionBuff("스턴", 2f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 50);
            } 
        }
        else if (playerAttackType == EAttakcType.Slash)
        {
            isTryLanding = true;

            if (CheckWall(dir, out RaycastHit hit, dist)) //기절
            {
                end = SnapToGrid(hit.transform.position - dir);
                KnockbackBeforeStatus = new ExhaustionBuff("기절", 4f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 100);
            }
            else // 스턴
            {
                KnockbackBeforeStatus = new ExhaustionBuff("스턴", 2f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 50);
            }
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float yOffset = 4 * height * t * (1 - t); // 포물선 공식
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += yOffset;
            transform.position = pos;

            if (t >= landingTime)
                isFalling = true;

            RotateTowardsDirection(-dir);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        targetPosition = transform.position;

        yield return new WaitForEndOfFrame();
        IsKnockedBack = false;


        fsmMachine.ChangeState(KnockbackBeforeState);
        AddEffect(KnockbackBeforeStatus);
    }


    public void AddEffect(StatusEffect effect)
    {
        if (effect == null)
            return;

        statusEffectManager.AddEffect(effect);
    }
    
    void IDamageAble.TakeDamage(float damage)
    {
        if (!isDamageAble) return;

        hp -= damage;

        if (hp <= 0)
        {
            Debug.Log("Die");
        }
    }
    

    void IEffectTarget.ModifyMoveSpeed(float factor)
    {
        baseMoveSpeed += factor;
    }
    void IEffectTarget.TakeTickDamage(float value)
    {
        ((IDamageAble)this).TakeDamage(value);
    }
    void IEffectTarget.SetDamaAble(bool value)
    {
        ResetInput();
        targetPosition = transform.position;

        isDamageAble = value;
    }
}
