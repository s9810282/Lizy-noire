using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPos;
    private bool isMoving = false;
    private Vector3 inputDir = Vector3.zero;

    void Start()
    {
        targetPos = SnapToGrid(transform.position);
        transform.position = targetPos;
    }

    public void Handle()
    {
        if (!isMoving && inputDir != Vector3.zero)
        {
            Vector3 nextPos = targetPos + inputDir;

            // 충돌 처리 나중에 → 지금은 무조건 이동
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

        if (inputDir != Vector3.zero)
            RotateTowardsDirection(inputDir);
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

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

    private void RotateTowardsDirection(Vector3 dir)
    {
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720 * Time.deltaTime);
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            Mathf.Round(pos.y),
            Mathf.Round(pos.z)
        );
    }
}
