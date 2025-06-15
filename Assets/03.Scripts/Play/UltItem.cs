using System.Collections;
using UnityEngine;


public interface IValueItem
{
    public float Execute();
}


public class UltItem : MonoBehaviour, IValueItem
{
    [SerializeField] int smallValue;
    [SerializeField] int largeValue;

    public float Execute()
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
