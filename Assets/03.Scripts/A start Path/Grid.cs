/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid<TGridObject> {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private float offset;
    private Vector3 originPosition;
    public TGridObject[,] gridArray;

    public Grid(int width, int height, float cellSize, Vector3Int originPosition, Func<Grid<TGridObject>, int, int, Vector3, TGridObject> createGridObject) {
        
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

       
        //MapScale 50
        //Map Size 10*10
        //CellSize 5
        //Offset = CellSize / 2

        //지금 PathNode x, y 값에 world 상의 x,z 값을 넣어야함 => 이게 아니 PathNode에 추가로 worldPos 변수 추가

        //start   -22.5 + 5*x 

        // orginPos.x + 2.5 + cellSize * x

        //왼쪽 맨아래 칸이 -22.5, -22.5 오른쪽 맨위가 22.5 ,22.5


        float originX = originPosition.x;
        float originZ = originPosition.z;

        //////DebugTool.Log("OriginX : " + originX);
        ////DebugTool.Log("OriginZ : " + originZ);

        //offset = (cellSize / 2);

        ////DebugTool.Log("Offset : " + offset);

        float tmpX = 0;
        float tmpZ = 0;


        for (int x = 0; x < gridArray.GetLength(0); x++) {
            
            tmpX = originX + offset + cellSize * x;

            for (int y = 0; y < gridArray.GetLength(1); y++) {
            
                tmpZ = originZ + offset + cellSize * y;
                gridArray[x, y] = createGridObject(this, x, y, new Vector3(tmpX, 0, tmpZ));
            }
        }


        bool showDebug = true;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) 
            {
                for (int y = 0; y < gridArray.GetLength(1); y++) 
                {                    
                    Debug.DrawLine(GetWorldPosition3D(x, 2, y), GetWorldPosition3D(x, 2, y + 1), Color.black, 100f);
                    Debug.DrawLine(GetWorldPosition3D(x, 2, y), GetWorldPosition3D(x + 1, 2 , y), Color.black, 100f);
                }
            }


            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetOriginPos()
    {
        return originPosition;
    }

    public Vector3 GetWorldPosition2D(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public Vector3 GetWorldPosition3D(int x, int y = 0, int z = 0)
    {
        return new Vector3(x, y, z) * cellSize + originPosition;
    }

    public void GetXY2D(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void GetXY3D(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            TriggerGridObjectChanged(x, y);
        }
    }

    public void TriggerGridObjectChanged(int x, int y) {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        GetXY2D(worldPosition, out int x, out int y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject2D(Vector3 worldPosition) {
        int x, y;
        GetXY2D(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public TGridObject GetGridObject3D(Vector3 worldPosition)
    {
        int x, y;
        GetXY3D(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }
}
