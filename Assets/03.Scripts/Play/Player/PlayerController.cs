using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IEffectTarget
{
    [Header("System")]
    [SerializeField] StateMachine fsmMachine;
    [SerializeField] StatusEffectManager statusEffectManager;
    [SerializeField] UltimateExecutor ultimateExecutor;
    [SerializeField] Animator anim;
    [SerializeField] GameObject playerMesh;

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
    [Header("Boost")]
    [SerializeField] private int maxboostCount = 3;
    [SerializeField] private int boostCount = 3;
    [SerializeField] private float boostRecoveryTime = 2;
    [SerializeField] private float boostRemindTime = 2;
    [Header("Ult")]
    [SerializeField] private int ultCount = 0;
    [SerializeField] private int ultMaxCount = 3;
    [SerializeField] private int ultValue = 0;
    [SerializeField] private int ultMaxValue = 1000;
    [SerializeField] private float firstUltTimer = 0f;
    [SerializeField] private float secondUltTimer = 0.75f;
    [SerializeField] private float thirdUltTimer = 1.5f;

    [Space(15f)]

    [Header("Player KnockBack")]
    [Tooltip("언제부터 착지 가능한가 0~1")][SerializeField] private float landingTime = 0.6f;
    [Tooltip("착지 성공 최소값")][SerializeField] private float landingMin = 1.2f;
    [Tooltip("착지 성공 최대값")][SerializeField] private float landingMax = 1.8f;
    [Tooltip("원한다면 직접 그리기")][SerializeField] AnimationCurve curve;
    [SerializeField] private float getDamageValue = 0;
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
    float boostRemindTimer;

    float ultChargeTimer;

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
        boostRemindTimer = 0;

        targetPosition = SnapToGrid(transform.position);
        transform.position = targetPosition;

        UpdateSpeed();
        UpdateUltValue(0);

        hp = maxHp;

        fsmMachine.ChangeState(new IdleState(this));
        anim.SetBool("isMove", false);

        EventBus.Publish(playerAttackType);
        EventBus.Publish(new BoostUIEvent { 
            boostCount = this.boostCount,
            timer = 0f,
            maxtimer = 0f
        });
        EventBus.Publish(new PlayerUIEvent
        {
            curHp = hp,
            maxHp = maxHp,
        });
    }

    private void Update()
    {
        fsmMachine.Update();
        statusEffectManager.Update();

        if (Input.GetMouseButtonDown(0))
        {
            if (playerAttackType == EAttakcType.Blow)
                playerAttackType = EAttakcType.Slash;
            else
                playerAttackType = EAttakcType.Blow;

            EventBus.Publish(playerAttackType);
        }

        ChargeUlt();
    }


    public void RecoveryBoost()
    {
        boostRemindTimer += Time.deltaTime;

        if (boostCount < maxboostCount)
        {
            if (boostRemindTimer < boostRemindTime) return;

            boostTimer += Time.deltaTime;

            EventBus.Publish(new BoostUIEvent
            {
                boostCount = this.boostCount,
                timer = boostTimer,
                maxtimer = boostRecoveryTime,
            });

            if (boostTimer > boostRecoveryTime) UpdateBoostCount(1);
        }
    }
    public void ChargeUlt()
    {
        if (IsKnockedBack) return;

        if (Input.GetMouseButtonDown(1))
        {
            ultChargeTimer = 0;
        }
        else if (Input.GetMouseButton(1))
        {
            ultChargeTimer += Time.deltaTime;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            int ult = ultChargeTimer > thirdUltTimer ? 3 : ultChargeTimer > secondUltTimer ? 2 : 1;

            if (ult > ultCount) ult = ultCount;

            UpdateUltCount(-ult);

            EventBus.Publish(new EffectRequest
            {
                effectCode = "UltAttack",
                type = EffectType.UltAttack,
                offset = transform.position,
                parent = null,
            });


            StartCoroutine(RemoveEffect("UltAttack", 0.5f));

            ultimateExecutor.ExcuteUlt(playerAttackType, ult, transform.position, transform.forward);
            ultChargeTimer = 0;


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
            Debug.Log("Good");

            KnockbackBeforeStatus = new SpeedBuff("부스트 속도 업", 3f, this, EStatusEffect.SpeedUp, baseMoveSpeed/2f);
            KnockbackBeforeState = new BoostState(this, 3f);
        }
        else //경직
        {
            Debug.Log("Bad Late");
            KnockbackBeforeStatus = new ExhaustionBuff("경직", 2f, this, EStatusEffect.Exhaustion, baseMoveSpeed, 25);
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

    public void StartKnockback(Vector3 direction, Monster target, bool isBoost = false, float boostDuration = 0f, float distance = 2f, float height = 1f, float duration = 0.4f)
    {
        ultChargeTimer = 0;

        RotateInstantly(-direction);

        bool isSpace = true;

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


        if (statusEffectManager.CheckStatus(EStatusEffect.Exhaustion))
        {
            isSpace = false;

            target.AttackPlayer();
            TakeDamage(getDamageValue);

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
            isSpace = true;
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

                target.RemoveShield(toPlayer);
                target.TakeDamage(atk, playerAttackType);

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


                if (!target.CheckShield(toPlayer))
                {
                    IsKnockedBack = false;
                    target.TakeDamage(9999, playerAttackType);
                    
                    transform.position = targetPosition;
                    if (!isBoost)
                        fsmMachine.ChangeState(new MoveState(this));
                    else
                    {
                        UpdateBoostCount(1);
                        fsmMachine.ChangeState(new BoostState(this, 3f, boostDuration));
                        statusEffectManager.UpdateEffectDuraton(EStatusEffect.SpeedUp, 1.5f);
                        EventBus.Publish(new SlashHitEvent { revoverValue = 1.5f });
                    }

                    return;
                }
                else
                {
                    target.TakeDamage(0, playerAttackType);
                   
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


        target.RotateInstantly(direction);
        ResetBoostTimer();

        StartCoroutine(DoParabolaKnockback(direction, distance, height, duration , start, end, isSpace));
    }
    private IEnumerator DoParabolaKnockback(Vector3 dir, float dist, float height, float duration, Vector3 start, Vector3 end, bool isSpace)
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

            if (isSpace)
            {
                if (t >= landingTime && !isFalling)
                {
                    isFalling = true;
                    EventBus.Publish(new SpaceToggleEvent());
                }
            }
            RotateTowardsDirection(-dir);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        targetPosition = transform.position;
        IsKnockedBack = false;

        fsmMachine.ChangeState(KnockbackBeforeState);
        statusEffectManager.AddEffect(KnockbackBeforeStatus);
        EventBus.Publish(new SpaceToggleEvent());
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
 
    public int UpdateBoostCount(int value)
    {
        boostTimer = 0;
        boostCount += value;

        EventBus.Publish(new BoostUIEvent
        {
            boostCount = this.boostCount,
            timer = boostTimer,
            maxtimer = boostRecoveryTime,
        });

        if (boostCount < 0) boostCount = 0;
        if (boostCount > maxboostCount) boostCount = maxboostCount;

        return boostCount;
    }
    public void ResetBoostTimer()
    {
        boostRemindTimer = 0;
        boostTimer = 0;

        UpdateBoostCount(0);
    }
    
    public int UpdateUltValue(int value)
    {
        if (value == -1) return 0;
        if (ultCount == ultMaxCount) return ultValue;

        //ultChargeTimer = 0;
        ultValue += value;

        if(ultValue >= ultMaxValue)
        {
            ultValue -= ultMaxValue;
            UpdateUltCount(1);

            if (ultCount == ultMaxCount)
                ultValue = 0;
        }
     
        EventBus.Publish(new UltUIEvent
        {
            ultCount = this.ultCount,
            ultGage = ultValue,
            ultMaxGage = ultMaxValue, 
        });

        return ultValue;
    }
    public int UpdateUltCount(int value)
    {
        ultChargeTimer = 0;
        ultCount += value;

        if (ultCount < 0) ultCount = 0;
        if (ultCount > ultMaxCount) ultCount = maxboostCount;

        EventBus.Publish(new UltUIEvent
        {
            ultCount = this.ultCount,
            ultGage = ultValue,
            ultMaxGage = ultMaxValue,
        });

        return ultCount;
    }

    #endregion



    #region IDamage

    void TakeDamage(float damage)
    {
        hp -= damage;

        EventBus.Publish(new PlayerUIEvent
        {
            curHp = hp,
            maxHp = maxHp,
        });

        if (hp <= 0)
        {
            Debug.Log("Die");
            fsmMachine.ChangeState(new DeadState(this));
        }
    }
    public void SetMeshActive(bool isActive)
    {
        playerMesh.gameObject.SetActive(isActive);
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
        TakeDamage(value);
    }
    void IEffectTarget.SetDamageValue(float value)
    {
        ResetInput();
        targetPosition = transform.position;

        getDamageValue = value;
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
