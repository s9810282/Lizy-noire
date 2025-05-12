using UnityEngine;
using UnityEngine.Experimental.AI;

public class MapLoader : MonoBehaviour
{
    [SerializeField] MapData map;

    void Start()
    {
        PathFinder.Instance.mapData = map;
        PathFinder.Instance.Test();
        PathFinder.Instance.SetClosedList();
    }

    public void SetMapData(MapData _mapData)
    {
        map = _mapData;
        PathFinder.Instance.Init(_mapData);

        //¸Ê »ý¼ºÇÏ±â

    }
}
