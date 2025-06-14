using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EMapType
{ 
    Stage_1,
    Stage_2,
}


[System.Serializable]
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
    Monster_3,

    BlueCrystal_1,
    BlueCrystal_2,
    BlueCrystal_3,

    BlueDoor_1,
    BlueDoor_2,
    BlueDoor_3,

    RedCrystal_1,
    RedCrystal_2,
    RedCrystal_3,

    RedDoor_1,
    RedDoor_2,
    RedDoor_3,

    END,
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
    public Vector3Int mapMinSize;
    public Vector3Int mapMaxSize;

    public List<MapTile> tiles = new List<MapTile>();
    public List<MapTile> bgTiles = new List<MapTile>();
}
