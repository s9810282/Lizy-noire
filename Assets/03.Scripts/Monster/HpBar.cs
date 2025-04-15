using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 2, 0);

    [SerializeField] Image fillImage;

    [SerializeField] Camera camera;

    void Start()
    {
        camera = Camera.main;    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
    }

    public void SetHP(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}
