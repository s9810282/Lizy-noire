using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;

    public int x;
    public int y;

    public Vector3 worldPosition;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;



    public PathNode(Grid<PathNode> grid, int x, int y, Vector3 pos, bool isWalkable = true)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;

        worldPosition = pos;
    }


    public Vector3 ReturnWorldPosition()
    {
        return worldPosition;
    }
    public void SetWorldPos(Vector3 pos)
    {
        worldPosition = pos;
    }

    public void SetIsWalkAble(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
