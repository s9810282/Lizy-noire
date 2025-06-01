using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IEffectTarget, IDamageAble
{
    [Header("System")]
    [SerializeField] StateMachine fsmMachine;
    [SerializeField] StatusEffectManager statusEffectManager;
    [SerializeField] Animator anim;

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
    [SerializeField] private int maxboostCount = 3;
    [SerializeField] private int boostCount = 3;
    [SerializeField] private float boostRecoveryTime = 2;
    [SerializeField] private int getDamageValue = 0;

    [Space(15f)]

    [Header("Player KnockBack")]
    [Tooltip("언제부터 착지 가능한가 0~1")][SerializeField] private float landingTime = 0.6f;
    [Tooltip("착지 성공 최소값")][SerializeField] private float landingMin = 1.2f;
    [Tooltip("착지 성공 최대값")][SerializeField] private float landingMax = 1.8f;
    [Tooltip("원한다면 직접 그리기")][SerializeField] AnimationCurve curve;
    [Space(15f)]
    [SerializeField] private float knockBackDuration = 0.25f;
    [SerializeField] private float knockBackHeight = 1f;
    [Space(15f)]
    [SerializeField] bool isKnockedBack = false;
    [SerializeField] private bool isTryLanding = false;
    [SerializeField] private bool isFalling = false;

    [SerializeField] public bool isInvinvible => statusEffectManager.CheckStatus(EStatusEffect.Invincible);

    [Header("Layers")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask bounceLayer;

    [Header("Field")]
    [SerializeField] Vector3 inputDirection = Vector3.zero;
    [SerializeField] Vector3 targetPosition;



    float boostTimer;

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
    public EPlayerState PlayerState { get => playerState; set => playerState = value; }


    #endregion


    private void Awake()
    {
        fsmMachine = new StateMachine();
        statusEffectManager = new StatusEffectManager();
    }

    private void Start()
    {
        boostCount = maxboostCount;
        boostTimer = 0;

        targetPosition = SnapToGrid(transform.position);
        transform.position = targetPosition;

        UpdateSpeed();

        hp = maxHp;

        fsmMachine.ChangeState(new IdleState(this));
        anim.SetBool("isMove", false);
        
    }

    private void Update()
    {
        fsmMachine.Update();
        statusEffectManager.Update();


        if(Input.GetMouseButtonDown(0))
        {
            if (playerAttackType == EAttakcType.Blow)
                playerAttackType = EAttakcType.Slash;
            else
                playerAttackType = EAttakcType.Blow;
        }

        if(boostCount < maxboostCount)
        {
            boostTimer += Time.deltaTime;

            if(boostTimer > boostRecoveryTime)
            {
                UpdateBoostCount(1);
                boostTimer = 0;
            }
        }

    }


    #region Input
    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsKnockedBack) return;
        if (currentMoveSpeed <= 0)
        {
            ResetInput();
            return;
        }

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
            Debug.Log("Bad Early");
            KnockbackBeforeStatus = new ExhaustionBuff("경직", 2f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 25);
        }
        else if (yPos > landingMin && boostCount > 0) //부스트
        {
            //부스트 활성화 시 끝나는 것도 체크해야함
            //부스트 남은 횟수, 그에 따른 시간 등 체크
            Debug.Log("Good");

            boostTimer = 0;
            UpdateBoostCount(-1);
            
            KnockbackBeforeStatus = new SpeedBuff("부스트 속도 업", 3f, this, EStatusEffect.SpeedUp, baseMoveSpeed/2f);
            KnockbackBeforeState = new BoostState(this, 3f, isExhaustion:boostCount == 0);
        }
        else //경직
        {
            Debug.Log("Bad Late");
            
        }
    }

    public void ResetInput()
    {
        InputDirection = Vector3.zero;
    }

    #endregion


    #region Position

    public void SetTargetPosition(Vector3 worldPos)
    {
        targetPosition = SnapToGrid(worldPos);
    }
    public void SetPlayerPosToTarget()
    {
        transform.position = targetPosition;
    }   /*SetTargetPosition 호출 후 사용*/
    public Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            1f,
            Mathf.Round(pos.z)
        );
    }
    public Vector3 SnapToGridZero(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            0f,
            Mathf.Round(pos.z)
        );
    }
    public Vector3 GetPlayerToTargetFoward()
    {
        return targetPosition - SnapToGrid(transform.position);
    }

    #endregion


    #region Rotate

    public void RotateTowardsDirection()
    {
        if (InputDirection == Vector3.zero) return;
        if (currentMoveSpeed <= 0)
        {
            ResetInput();
            return;
        }

        float yRotation = DirToYRotation(InputDirection);
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    public void RotateTowardsDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        if (currentMoveSpeed <= 0)
        {
            ResetInput();
            return;
        }

        float yRotation = DirToYRotation(dir);
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    public void RotateInstantly(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        if (currentMoveSpeed <= 0) 
        { 
            ResetInput(); 
            return; 
        }

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

    #endregion


    #region Raycast/KnockBack

    public bool RaycaseWall(Vector3 dir, out RaycastHit hit, float distance = 0.8f)
    {
        return Physics.Raycast(transform.position, dir, out hit, distance, wallLayer);
    }
    public bool RaycaseBounce(Vector3 dir, out RaycastHit hit)
    {
        if (statusEffectManager.CheckStatus(EStatusEffect.Invincible))
        {
            hit = default;
            return false;
        }
        
        Vector3 pos = transform.position;

        return Physics.Raycast(pos, dir, out hit, 0.8f, bounceLayer);
    }


    //Monster Data에 값에 따라서 값들 조정
    StatusEffect KnockbackBeforeStatus;
    IState KnockbackBeforeState;

    public void StartKnockback(Vector3 direction, Monster target, float distance = 2f, float height = 1f, float duration = 0.4f)
    {
        RotateInstantly(-direction);
        
        IsKnockedBack = true;
        isTryLanding = false;
        isFalling = false;

        distance = target.Data.monsterValue.bounceDistance;
        duration = knockBackDuration * distance;
        height = knockBackHeight;

        KnockbackBeforeState = new IdleState(this);
        KnockbackBeforeStatus = new ExhaustionBuff("경직", 1f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 25);
        
        Vector3 start = transform.position;
        Vector3 end = SnapToGrid(start + direction * distance);
        Vector3 wallFront = Vector3.zero;
        direction = SnapToGridZero(direction);

        target.RotateInstantly(direction);

        if (statusEffectManager.CheckStatus(EStatusEffect.Exhaustion))
        {
            target.AttackPlayer();
            ((IDamageAble)this).TakeDamage(target.Data.damage);

            distance = 1f;
            isTryLanding = true;
            KnockbackBeforeStatus = new InvincibleBuff("무적", 5f, this, EStatusEffect.Invincible);

            if (RaycaseWall(direction, out RaycastHit hit, distance))
            {
                AnimSetInt("KnockBack", 1);

                direction *= 2;
                Debug.Log(direction);
                wallFront = SnapToGrid(hit.transform.position - direction);
                float b = Vector3.Distance(start, wallFront) + 1f;
                duration = b * knockBackDuration;
                end = wallFront;
            }
            else
            {
                AnimSetInt("KnockBack", 1);
                duration = knockBackDuration;
                end = SnapToGrid(start + direction * distance);
            }
        }
        else
        {
            Vector3 toPlayer = (transform.position - target.transform.position).normalized;

            if (playerAttackType == EAttakcType.Blow)
            {
                AnimSetInt("KnockBack", 1);

                EventBus.Publish(new EffectRequest
                {
                    effectCode = "PlayerBlow",
                    type = EffectType.Blow,
                    offset = transform.position,
                    parent = null,
                });

                StartCoroutine(RemoveEffect("PlayerBlow", 0.5f));



                EventBus.Publish(new EffectRequest
                {
                    effectCode = "MonsterBlueSpark" + target.name,
                    type = EffectType.BlowSpark,
                    offset = target.transform.position,
                    parent = null,
                });

                StartCoroutine(RemoveEffect("MonsterBlueSpark" + target.name, 0.5f));

                target.RemoveShield(toPlayer);
                target.TakeDamage(atk);

                if (RaycaseWall(direction, out RaycastHit hit, distance)) //스턴
                {
                    wallFront = SnapToGrid(hit.transform.position - direction);
                    float b = Vector3.Distance(start, wallFront) + 1f;
                    duration = b * knockBackDuration;
                    end = wallFront;

                    KnockbackBeforeStatus = new ExhaustionBuff("스턴", 2f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 50, true);
                }
                else
                {
                    end = SnapToGrid(start + direction * distance);
                }
            }
            else if(playerAttackType == EAttakcType.Slash)
            {
                EventBus.Publish(new EffectRequest
                {
                    effectCode = "PlayerSlash",
                    type = EffectType.Slash,
                    offset = transform.position,
                    parent = null,
                });

                StartCoroutine(RemoveEffect("PlayerSlash", 0.5f));



                EventBus.Publish(new EffectRequest
                {
                    effectCode = "MonsterRedSpark" + target.name,
                    type = EffectType.SlashSpark,
                    offset = target.transform.position,
                    parent = null,
                });

                StartCoroutine(RemoveEffect("MonsterRedSpark" + target.name, 0.5f));


                if (!target.CheckShield(toPlayer))
                {
                    EventBus.Publish(new EffectRequest
                    {
                        effectCode = "PlayerSlashHit",
                        type = EffectType.SlashHit,
                        offset = transform.position,
                        parent = null,
                    });

                    StartCoroutine(RemoveEffect("PlayerSlashHit", 0.5f));

                    IsKnockedBack = false;
                    target.TakeDamage(9999);

                    ResetInput();
                    fsmMachine.ChangeState(new IdleState(this));
                    return;
                }
                else
                {
                    //AnimSetInt("KnockBack", 2);

                    isTryLanding = true;

                    height = 0.25f;
                    distance = distance * 1.5f;
                    duration = duration / 2;
                    
                    if (RaycaseWall(direction, out RaycastHit hit, distance)) //기절
                    {
                        wallFront = SnapToGrid(hit.transform.position - direction);
                        float b = Vector3.Distance(start, wallFront) + 1f;
                        duration = b * knockBackDuration / 2;
                        end = wallFront;

                        if (b > 2f)
                            AnimSetInt("KnockBack", 2);
                        else
                            AnimSetInt("KnockBack", 1);

                        KnockbackBeforeStatus = new ExhaustionBuff("기절", 4f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 100, true);
                    }
                    else // 스턴
                    {
                        AnimSetInt("KnockBack", 2);
                        end = SnapToGrid(start + direction * distance);
                        KnockbackBeforeStatus = new ExhaustionBuff("스턴", 2f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 50);
                    }
                }
            }
        }

        StartCoroutine(DoParabolaKnockback(direction, distance, height, duration , start, end));
    }
    private IEnumerator DoParabolaKnockback(Vector3 dir, float dist, float height, float duration, Vector3 start, Vector3 end)
    {
        ResetInput();

        float elapsed = 0f;

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
        IsKnockedBack = false;

        fsmMachine.ChangeState(KnockbackBeforeState);
        statusEffectManager.AddEffect(KnockbackBeforeStatus);   
    }

    #endregion
    

    #region State

    public void AddEffect(StatusEffect effect)
    {
        statusEffectManager.AddEffect(effect);
    }
    public bool CheckEffect(EStatusEffect effect)
    {
        return statusEffectManager.CheckStatus(effect);
    }

    public void UpdateSpeed()
    {
        currentMoveSpeed = baseMoveSpeed + moveSpeedModifier;
        anim.SetFloat("Speed", currentMoveSpeed);
    }
    public void PlayAnim(string animName, float speed = 1f)
    {
        anim.Play(animName, 0, speed);
    }
    public void AnimSetBool(string id, bool type = false)
    {
        anim.SetBool(id, type);
    }
    public void AnimSetInt(string id, int type = 1)
    {
        anim.SetInteger(id, type);
    }
    public void UpdateGetDamageValue(int value)
    { 
        getDamageValue = value;
    }
    public void UpdateBoostCount(int value)
    {
        boostCount += value;

        if (boostCount < 0) boostCount = 0;
        if (boostCount > maxboostCount) boostCount = maxboostCount;
    }

    #endregion



    #region IDamage

    void IDamageAble.TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Debug.Log("Die");
            fsmMachine.ChangeState(new DeadState(this));
        }
    }

    #endregion


    #region IEffect

    void IEffectTarget.ModifyMoveSpeed(float factor)
    {
        moveSpeedModifier += factor;
        UpdateSpeed();
    }
    void IEffectTarget.TakeTickDamage(float value)
    {
        ((IDamageAble)this).TakeDamage(value);
    }
    void IEffectTarget.SetDamaAble(bool value)
    {
        ResetInput();
        targetPosition = transform.position;
    }
    public Transform GetTarget()
    {
        return transform;
    }

    IEnumerator RemoveEffect(string effectcode, float duration)
    {
        yield return new WaitForSeconds(duration);
        EventBus.Publish(effectcode);
    }

    #endregion
}
