using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Interfaces;

public class PlayerController : MonoBehaviour
{
    [SerializeField] StateMachine fsmMachine;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 1080f;

    [SerializeField] bool isKnockedBack;
    [SerializeField] bool isTryLanding = false;
    [SerializeField] bool isfalling = false;

    [Header("Layers")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask bounceLayer;

    [Header("Field")]
    [SerializeField] Vector3 inputDirection = Vector3.zero;
    [SerializeField] Vector3 targetPosition;


    #region Property

    public StateMachine FSMMachine { get => fsmMachine; set => fsmMachine = value; }

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }

    public Vector3 InputDirection { get => inputDirection; set => inputDirection = value; }
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    
    public bool IsKnockedBack { get => isKnockedBack; set => isKnockedBack = value; }

    #endregion

    private void Awake()
    {
        fsmMachine = new StateMachine();
    }

    private void Start()
    {
        targetPosition = SnapToGrid(transform.position);
        transform.position = targetPosition;

        fsmMachine.ChangeState(new IdleState(this));
    }

    private void Update()
    {
        fsmMachine.Update();
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
        if (isTryLanding) return;
        if (!isfalling) return;

        Debug.Log("Space");

        isTryLanding = true;

        float yPos = transform.position.y;

        if (yPos > 1.8f)  //경직
        {
            Debug.Log("Bad");
            KnockbackChangeState = new IdleState(this);
        }
        else if (yPos > 1.2f) //부스트
        {
            Debug.Log("Good");
            KnockbackChangeState = new IdleState(this);
        }
        else //경직
        {
            Debug.Log("Bad");
            KnockbackChangeState = new IdleState(this);
        }
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

    public void StartKnockback(Vector3 direction, float distance = 2f, float height = 1f, float duration = 0.4f)
    {
        if (IsKnockedBack) return;

        StartCoroutine(DoParabolaKnockback(direction, distance, height, duration));
    }



    IState KnockbackChangeState;
    private IEnumerator DoParabolaKnockback(Vector3 dir, float dist, float height, float duration)
    {
        IsKnockedBack = true;

        KnockbackChangeState = new IdleState(this);

        Vector3 start = transform.position;
        Vector3 end = SnapToGrid(start + dir * dist);
        float elapsed = 0f;

        ResetInput();

        if (CheckWall(dir, out RaycastHit hit, 2f))
        {
            end = SnapToGrid(hit.transform.position - dir);

            //여기서 참격인지 타격인지에 따라서 스턴 or 기절 판정, 벽 ㄹ판정보다 스턴.기절 구분이 먼저
        }

        isTryLanding = false;
        isfalling = false;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float yOffset = 4 * height * t * (1 - t); // 포물선 공식
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += yOffset;
            transform.position = pos;

            if(t >= 0.5f)
                isfalling = true;

            RotateTowardsDirection(-dir);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        targetPosition = transform.position;

        yield return new WaitForEndOfFrame();
        IsKnockedBack = false;

        if(!isTryLanding)
            FSMMachine.ChangeState(new IdleState(this)); // 인풋 없을경우 자세붕괴
    }
}
