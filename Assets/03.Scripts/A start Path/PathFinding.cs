using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public Grid<PathNode> grid;

    public List<PathNode> openList;
    public List<PathNode> closedList;

    public Vector3Int originOffeset;
    public int offsetWidth;
    public int offsetHeight;


    public List<PathNode> mapClosedList = new List<PathNode>();

    public PathFinding(int width, int heigh, float cellSize, Vector3Int originPos)
    {
        offsetWidth = width;
        offsetHeight = heigh;
        originOffeset = originPos;

        grid = new Grid<PathNode>(width, heigh, cellSize, originPos, (Grid<PathNode> grid, int x, int y, Vector3 pos) => new PathNode(grid, x, y, pos));
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        startX -= originOffeset.x;
        startY -= originOffeset.z;

        endX -= originOffeset.x;
        endY -= originOffeset.z;

        //Debug.Log(startX + " " + startY);
        
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>(); //이거 여기서 미리 추가해주기 MapLoader에서 벽들값을 가져와서
        closedList.AddRange(mapClosedList);

        Debug.Log("Open List :  " + openList.Count);
        Debug.Log("Closed List :  " + closedList.Count);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                pathNode.gCost = int.MaxValue;
                pathNode.hCost = 0;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CaculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowerstFCostNode(openList);

            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode node in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(node)) continue;
                if (!node.isWalkable)
                {
                    closedList.Add(node);
                    continue;
                }

                int tempGCost = currentNode.gCost + CaculateDistance(currentNode, node);

                if(tempGCost < node.gCost)
                {
                    node.cameFromNode = currentNode;
                    node.gCost = tempGCost;
                    node.hCost = CaculateDistance(node, endNode);
                    node.CalculateFCost();

                    if (!openList.Contains(node)) openList.Add(node);

                }
            }
        }

        return null;
    }

    public List<PathNode> GetNeighbourList(PathNode currentNode) //GetNeighbour All Node (8 Node)  
    {
        List<PathNode> neighbourList = new List<PathNode>();

        //float minX = grid.GetOriginPos().x;// + grid.GetCellSize() / 2;
        //float minZ = grid.GetOriginPos().z;// + grid.GetCellSize() / 2;
        //float maxX = Mathf.Abs(grid.GetOriginPos().x);
        //float maxZ = Mathf.Abs(grid.GetOriginPos().z);
        //float cellSize = grid.GetCellSize();


        //이 방식은 사실상 Index로 하는거나 다름이 없음
        if (currentNode.x - 1 >= 0)  //current Node x - 1 => grid.MaxX 
        {
            //Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));

            //Left Down
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));

            //LeftUp
            //if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));

        }

        if (currentNode.x + 1 < grid.GetWidth()) //current Node x + 1 < grid.EndX
        {
            //Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));

            //Right Down
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));

            //Right Up
            //if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        //Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));

        //Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));


        return neighbourList;
    }



    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public PathNode GetNode(Vector3 pos)
    {
        return grid.GetGridObject2D(pos);
    }


    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);

        PathNode currentNode = endNode;

        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    public int CaculateDistance(PathNode a, PathNode b)
    {
        int xDistnace = Mathf.Abs(a.x - b.x);
        int yDistnace = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistnace - yDistnace);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistnace, yDistnace) + MOVE_STRAIGHT_COST * remaining;
    }

    public PathNode GetLowerstFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestNode = pathNodeList[0];

        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowestNode.fCost)
            {
                lowestNode = pathNodeList[i];
            }
        }

        return lowestNode;
    }
}
