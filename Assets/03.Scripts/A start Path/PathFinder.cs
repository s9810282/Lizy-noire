using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] PathFinding pathFinding;
    [SerializeField] Vector3Int mapMinSize;
    [SerializeField] Vector3Int mapMaxSize;

    // Start is called before the first frame update
    void Start()
    {
        pathFinding = new PathFinding
            ((int)(mapMaxSize.x - mapMinSize.x) + 1, (int)(mapMaxSize.z - mapMinSize.z) + 1, 1f, mapMinSize);
        

        List<PathNode> path = pathFinding.FindPath(-15, -8,  6, 11);

        if (path != null)
        {
            Debug.DrawLine(new Vector3(mapMinSize.x, 2, mapMinSize.z) * 1f + Vector3.one * 0f,
                 new Vector3(path[0].worldPosition.x, 2, path[0].worldPosition.z) * 1f + Vector3.one * 0f, Color.red, 100f);

            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.Log(path[i].worldPosition.x + ", " + path[i].worldPosition.z);

                Debug.DrawLine(new Vector3(path[i].worldPosition.x, 2, path[i].worldPosition.z) * 1f + Vector3.one * 0f, 
                    new Vector3(path[i + 1].worldPosition.x, 2, path[i + 1].worldPosition.z) * 1f + Vector3.one * 0f, Color.red, 100f);
            }
        }
    }
}

