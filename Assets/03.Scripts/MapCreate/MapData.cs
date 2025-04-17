using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


[Serializable]
public enum ETileType
{
    None = 0,
    START,

    Floor_1,
    Floor_2,
    Floor_3,

    Wall_1,
    Wall_2,
    Wall_3,
    Wall_4,
    Wall_5,
    Wall_6,

    B_Wall_1,
    B_Wall_2,
    B_Wall_3,


    Chest_1,

    Potion_1,

    Moneter_1,
    Moneter_2,

}

[Serializable]
public class MapTile
{
    public ETileType TileType;

    public int x;
    public int z;

    public MapTile(int x, int y)
    {
        this.x = x;
        this.z = y;
    }

    public MapTile(int x, int z, ETileType tileType)
    {
        TileType = tileType;
        this.x = x;
        this.z = z;
    }
}


[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public string stageName;
    public Vector2 mapSize;

    public List<MapTile> tiles = new List<MapTile>();
    public List<MapTile> bgTiles = new List<MapTile>();
}
