using UnityEngine;
using UnityEngine.InputSystem;

public class ActionTest : MonoBehaviour
{
    private Vector2 moveInput;
    public float moveSpeed = 5f;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime);
    }
}
