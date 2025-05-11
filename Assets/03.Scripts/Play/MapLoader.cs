using UnityEngine;
using UnityEngine.Experimental.AI;

public class MapLoader : MonoBehaviour
{
    void Start()
    {
        PathFinder.Instance.Test();
    }

    public static void SetMapData(MapData _mapData)
    {
        PathFinder.Instance.Init(_mapData);
    }
}
