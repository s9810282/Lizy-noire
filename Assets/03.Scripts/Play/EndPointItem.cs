using UnityEngine;

public class EndPointItem : MonoBehaviour, IValueItem
{
    
    void Start()
    {
        
    }

    public float Execute()
    {
        if(GameManager.Instance.curMapType == EMapType.Stage_2)
        {
            EventBus.Publish(new StageResultEvent { e = EStageResultType.Clear });
        }
        else
        {
            EventBus.Publish(new StageResultEvent { e = EStageResultType.Next });
        }

        return -1;
    }
}
