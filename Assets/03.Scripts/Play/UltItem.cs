using System.Collections;
using UnityEngine;


public interface IValueItem
{
    public float GetValue();
}


public class UltItem : MonoBehaviour, IValueItem
{
    [SerializeField] int smallValue;
    [SerializeField] int largeValue;

    public float GetValue()
    {
        int val = Random.Range(smallValue, largeValue);
        Debug.Log("Item :" + val);

        Destroy(gameObject);
        return val;
    }

    private IEnumerator DestroyNextFrame()
    {
        yield return null;
        Destroy(gameObject);
    }
}
