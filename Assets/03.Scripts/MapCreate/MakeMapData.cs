using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[System.Serializable]
public class TileSpriteEntry
{
    public ETileType key;
    public Tile tile;
}

[System.Serializable]
public class TileCubeEntry
{
    public ETileType key;
    public GameObject cube;
}


public class MakeMapData : MonoBehaviour
{
    public Vector2Int startPos;
    public Vector2Int endPos;

    public Tilemap bgTilemap;
    public Tilemap tilemap;
    public MapData mapData;

    public GameObject tileBGParent;
    public GameObject tileParent;

    public List<TileSpriteEntry> tileSpritesList = new();

    private Dictionary<ETileType, Tile> tileSprites;
    public Dictionary<ETileType, Tile> GetTileSprites()
    {
        if (tileSprites == null || tileSprites.Count != tileSpritesList.Count)
        {
            tileSprites = tileSpritesList.ToDictionary(entry => entry.key, entry => entry.tile);
        }
        return tileSprites;
    }


    public List<TileCubeEntry> tileCubesList = new();

    private Dictionary<ETileType, GameObject> tileCubes;
    public Dictionary<ETileType, GameObject> GetTileCubes()
    {
        if (tileCubes == null || tileCubes.Count != tileCubes.Count)
        {
            tileCubes = tileCubesList.ToDictionary(entry => entry.key, entry => entry.cube);
        }
        return tileCubes;
    }


    public void Save()
    {
        ClearData();

        mapData.tiles = new List<MapTile>();
        mapData.bgTiles = new List<MapTile>();

       for(int i = startPos.y; i <= endPos.y; i++)
        {
            for(int j = startPos.x; j <= endPos.x; j++ )
            {
                MapTile tile = new MapTile(j, i, CheckBGTileType(j, i));
                MapTile tile2 = new MapTile(j, i, CheckTileType(j, i));

                mapData.bgTiles.Add(tile);
                mapData.tiles.Add(tile2);
            }
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }



    public ETileType CheckBGTileType(int x, int y)
    {
        Vector3Int cellPos = new Vector3Int(x, y, 0); // 원하는 타일 좌표
        TileBase tilebase = bgTilemap.GetTile(cellPos);
        Tile tile = tilebase as Tile;

        if (tile == null) return ETileType.None;
        if (tile.sprite == null) return ETileType.None;

        for (int i = 0; i < tileSpritesList.Count; i++)
        {
            if (tile == tileSpritesList[i].tile)
            {
                Debug.Log("Check BG");
                return tileSpritesList[i].key;
            }
        }

        return ETileType.None;
    }

    public ETileType CheckTileType(int x, int y)
    {
        Vector3Int cellPos = new Vector3Int(x, y, 0); // 원하는 타일 좌표
        TileBase tilebase = tilemap.GetTile(cellPos);
        Tile tile = tilebase as Tile;

        if (tile == null) return ETileType.None;
        if (tile.sprite == null) return ETileType.None;

        for(int i = 0; i < tileSpritesList.Count; i++)
        {
            if (tile == tileSpritesList[i].tile)
            {
                Debug.Log("Check Map");
                return tileSpritesList[i].key;
            }
        }

        return ETileType.None;
    }


    public GameObject GetCube(ETileType type)
    {
        return tileCubes[type];
    }

    public Sprite GetTileSprite(ETileType type)
    {
        return tileSprites[type].sprite;
    }


    public void LoadTileMap()
    {
        ClearTileMap();

        List<MapTile> bgTiles = mapData.bgTiles;
        List<MapTile> tiles = mapData.tiles;
        GetTileSprites();
        GetTileCubes();


        for (int i = 0; i < bgTiles.Count; i++)
        {
            if (bgTiles[i].TileType == ETileType.None) continue;

            Vector3Int cellPos = new Vector3Int(bgTiles[i].x, bgTiles[i].z, 0); // 원하는 타일 좌표

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = GetTileSprite(bgTiles[i].TileType);

            //bgTilemap.SetTile(cellPos, tile);
            bgTilemap.SetTile(cellPos, tileSprites[bgTiles[i].TileType]); 
        }



        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].TileType == ETileType.None) continue;

            Vector3Int cellPos = new Vector3Int(tiles[i].x, tiles[i].z, 0); // 원하는 타일 좌표

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = GetTileSprite(tiles[i].TileType);

            //tilemap.SetTile(cellPos, tile);
            tilemap.SetTile(cellPos, tileSprites[tiles[i].TileType]);
        }

    }

    public void LoadMap()
    {
        ClearMap();


        List<MapTile> bgTiles = mapData.bgTiles;
        List<MapTile> tiles = mapData.tiles;
        GetTileSprites();
        GetTileCubes();

        foreach (var item in bgTiles)
        {
            if (item.TileType == ETileType.None) continue;

            GameObject cube = Instantiate(GetCube(item.TileType));
            cube.transform.position = new Vector3(item.x, 0, item.z);
            cube.transform.parent = tileBGParent.transform;
        }


        foreach (var item in tiles)
        {
            if (item.TileType == ETileType.None) continue;

            GameObject cube = Instantiate(GetCube(item.TileType));
            cube.transform.position = new Vector3(item.x, 1, item.z);
            cube.transform.parent = tileParent.transform;
        }
    }

    public void ClearData()
    {
        mapData.bgTiles.Clear();
        mapData.tiles.Clear();

        Debug.Log("Clear");
    }

    public void ClearMap()
    {
        TagScript[] cubes = FindObjectsOfType<TagScript>();

        foreach (var item in cubes)
        {
            DestroyImmediate(item.gameObject);
        }
    }

    public void ClearTileMap()
    {
        bgTilemap.ClearAllTiles();
        tilemap.ClearAllTiles();
    }
}
