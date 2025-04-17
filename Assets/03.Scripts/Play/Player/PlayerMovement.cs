using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 1080f;

    [Space(10f)]

    [SerializeField] Vector3 targetPos;
    [SerializeField] Vector3 inputDir = Vector3.zero;

    [Space(10f)]

    [SerializeField] bool isMoving = false;
    [SerializeField] bool isKnockedBack = false;

    [Space(10f)]

    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask bounceLayer;

    void Start()
    {
        targetPos = SnapToGrid(transform.position);
        transform.position = targetPos;
    }

    public void Handle()
    {
        if (isKnockedBack) return;

        if (CheckWall(inputDir, out RaycastHit WallHit))
        {
            inputDir = Vector3.zero;
        }
        else if (CheckBounce(inputDir, out RaycastHit bounceHit))
        {
            KnockbackParabola(-inputDir);
            inputDir = Vector3.zero;
        }


        RotateTowardsDirection();
        Move();
    }


    public void Move()
    {

        if (!isMoving && inputDir != Vector3.zero)
        {
            Vector3 nextPos = targetPos + inputDir;

            targetPos = SnapToGrid(nextPos);
            isMoving = true;
        }
        
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
        }
    }
    public void OnMove(CallbackContext context)
    {
        if (isKnockedBack) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            inputDir = Vector3.zero;
            return;
        }

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            inputDir = input.x > 0 ? Vector3.right : Vector3.left;
        else
            inputDir = input.y > 0 ? Vector3.forward : Vector3.back;
    }



    public void RotateTowardsDirection()
    {
        if (inputDir == Vector3.zero)
            return;

        float yRotation = 0f;

        if (inputDir == Vector3.forward)
            yRotation = 0f;
        else if (inputDir == Vector3.right)
            yRotation = 90f;
        else if (inputDir == Vector3.back)
            yRotation = 180f;
        else if (inputDir == Vector3.left)
            yRotation = 270f;

        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    public void RotateTowardsDirection(Vector3 dir)
    {
        if (dir == Vector3.zero)
            return;

        float yRotation = 0f;

        if (dir == Vector3.forward)
            yRotation = 0f;
        else if (dir == Vector3.right)
            yRotation = 90f;
        else if (dir == Vector3.back)
            yRotation = 180f;
        else if (dir == Vector3.left)
            yRotation = 270f;

        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    public void RotateTowardsDirectionEuler()
    {
        if (inputDir == Vector3.zero)
            return;

        float yRotation = 0f;

        if (inputDir == Vector3.forward)
            yRotation = 0f;
        else if (inputDir == Vector3.right)
            yRotation = 90f;
        else if (inputDir == Vector3.back)
            yRotation = 180f;
        else if (inputDir == Vector3.left)
            yRotation = 270f;

        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = targetRotation;
    }



    public Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            1f,
            Mathf.Round(pos.z)
        );
    }


    public bool CheckWall(Vector3 dir, out RaycastHit hit, float distatnce = 0.8f)
    {
        
        return Physics.Raycast(transform.position, dir, out hit, distatnce, wallLayer);
    }
    public bool CheckBounce(Vector3 dir, out RaycastHit hit)
    {
        Vector3 vec = transform.position;
        vec.y = 1f;
        
        return Physics.Raycast(transform.position, dir, out hit, 0.5f, bounceLayer);
    }



    public void KnockbackParabola(Vector3 direction, float distance = 2f, float height = 1f, float duration = 0.4f)
    {
        if (isKnockedBack) return;

        StartCoroutine(DoParabolaKnockback(direction.normalized, distance, height, duration));
    }
    IEnumerator DoParabolaKnockback(Vector3 dir, float dist, float height, float duration)
    {
        isKnockedBack = true;

        Vector3 start = transform.position;
        Vector3 end = SnapToGrid(start + dir * dist);
        float elapsed = 0f;

        inputDir = Vector3.zero;


        if (CheckWall(dir, out RaycastHit WallHit, 2f))
        {
            end = SnapToGrid(WallHit.transform.position - dir);
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float yOffset = 4 * height * t * (1 - t); // Æ÷¹°¼± ±Ëµµ
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += yOffset;
            transform.position = pos;

            RotateTowardsDirection(-dir);

            elapsed += Time.deltaTime;
            yield return null;
        }

        inputDir = Vector3.zero;
        transform.position = end;
        targetPos = transform.position;

        yield return new WaitForEndOfFrame();
        isKnockedBack = false;
    }
}
