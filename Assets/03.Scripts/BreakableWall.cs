using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] ETileType type;
    [SerializeField] bool isCrystal = false;
    void Start()
    {
        
    }

    public void Break()
    {
        if (isCrystal)
        {
            EventBus.Publish(new CrystalBreakEvent
            {
                e = type
            });
        }
    }
}
