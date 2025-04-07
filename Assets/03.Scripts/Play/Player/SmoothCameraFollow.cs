using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 8f, -8f);  // 사선 위쪽에서 바라보는 시점
    public float followSpeed = 5f;
    public float rotationSpeed = 5f;

    [SerializeField] bool rotateLerp;

    private void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 desiredPosition = target.position + offset;

        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        if (rotateLerp)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
