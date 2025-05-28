using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 2, 0);

    [SerializeField] Image fillImage;
    [SerializeField] Image bgImage;

    [SerializeField] Camera camera;

    void Start()
    {
        camera = Camera.main;    
    }

    
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

    public void HideHpBar(bool isTrue)
    {
        gameObject.SetActive(isTrue);

        Color color = isTrue ? Color.white : Color.clear;

        fillImage.color = color;
        bgImage.color = color;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}
