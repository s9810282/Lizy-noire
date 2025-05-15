using System.Collections;
using System.Collections.Generic;
using Unity.Behavior.GraphFramework;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class PathFinder
{
    private static PathFinder instance;
    public static PathFinder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PathFinder();
            }

            return instance;
        }
    }

    public MapData mapData;

    public PathFinding pathFinding;
    public Vector3Int mapMinSize = new Vector3Int(-15, 0, -8);
    public Vector3Int mapMaxSize = new Vector3Int(6, 0, 11);

    public void Init(MapData map)
    {
        mapData = map;
        mapMinSize = mapData.mapMinSize;
        mapMaxSize = mapData.mapMaxSize;

        pathFinding = new PathFinding
            ((int)(mapMaxSize.x - mapMinSize.x) + 1, (int)(mapMaxSize.z - mapMinSize.z) + 1, 1f, mapMinSize);


        SetClosedList();
    }

    public void Test()
    {
        pathFinding = new PathFinding
            ((int)(mapMaxSize.x - mapMinSize.x) + 1, (int)(mapMaxSize.z - mapMinSize.z) + 1, 1f, mapMinSize);

        //List<PathNode> path = pathFinding.FindPath(-15, -8, 6, 11);

        //if (path != null)
        //{
        //    Debug.DrawLine(new Vector3(mapMinSize.x, 2, mapMinSize.z) * 1f + Vector3.one * 0f,
        //         new Vector3(path[0].worldPosition.x, 2, path[0].worldPosition.z) * 1f + Vector3.one * 0f, Color.red, 100f);

        //    for (int i = 0; i < path.Count - 1; i++)
        //    {
        //        Debug.Log(path[i].worldPosition.x + ", " + path[i].worldPosition.z);

        //        Debug.DrawLine(new Vector3(path[i].worldPosition.x, 2, path[i].worldPosition.z) * 1f + Vector3.one * 0f,
        //            new Vector3(path[i + 1].worldPosition.x, 2, path[i + 1].worldPosition.z) * 1f + Vector3.one * 0f, Color.red, 100f);
        //    }
        //}
    }


    public void SetClosedList()
    {
        pathFinding.mapClosedList = new List<PathNode>();

        foreach (MapTile item in mapData.tiles)
        {
            if (item == null) continue;
            if (item.TileType == ETileType.None) continue;
            if (item.TileType == ETileType.Moneter_1) continue;
            if (item.TileType == ETileType.Moneter_2) continue;

            PathNode current = pathFinding.grid.GetGridObject3D(new Vector3(item.x, 1, item.z));
            current.isWalkable = false;

            pathFinding.mapClosedList.Add(current);
        }
    }


    public List<PathNode> GetPathNodes(Vector3 startPos, Vector3 endPos)
    {
        List<PathNode> path = pathFinding.FindPath((int)startPos.x, (int)startPos.y, (int)endPos.x, (int)endPos.y);
        return path;
    }


    public List<PathNode> GetReachableNodes(PathNode startNode, int maxCost, int minCost = 0)
    {
        var open = new Queue<PathNode>();
        var visited = new Dictionary<PathNode, int>();
        var reachable = new List<PathNode>();

        open.Enqueue(startNode);
        visited[startNode] = 0;

        while (open.Count > 0)
        {
            var current = open.Dequeue();
            int currentCost = visited[current];

            if (currentCost >= minCost)
                reachable.Add(current);

            foreach (var neighbor in pathFinding.GetNeighbourList(current))
            {
                if (neighbor == null || !neighbor.isWalkable) continue;

                int costToNeighbor = currentCost + pathFinding.CaculateDistance(current, neighbor);

                if (costToNeighbor <= maxCost &&
                    (!visited.ContainsKey(neighbor) || costToNeighbor < visited[neighbor]))
                {
                    visited[neighbor] = costToNeighbor;
                    open.Enqueue(neighbor);
                }
            }
        }

        return reachable;
    }
}

