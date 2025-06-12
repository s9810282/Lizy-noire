using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] ETileType type;

    void Start()
    {
        
    }

    public void Break()
    {
        EventBus.Publish(new CrystalBreakEvent
        {
            e = type
        });
    }
}
