using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Experimental.AI;

public struct CrystalBreakEvent
{
    public ETileType e;
}


public class MapLoader : MonoBehaviour
{
    [SerializeField] MapData mapData;
    [SerializeField] List<TileCubeEntry> tileCubesList = new();
    [SerializeField] GameObject tileBGParent;
    [SerializeField] GameObject tileParent;

    [SerializeField] PlayerController player;
    [SerializeField] MonsterSpawner spawner;

    [SerializeField] bool isTest;

    private Dictionary<ETileType, GameObject> tileCubes;
    public Dictionary<ETileType, GameObject> GetTileCubes()
    {
        if (tileCubes == null || tileCubes.Count != tileCubes.Count)
        {
            tileCubes = tileCubesList.ToDictionary(entry => entry.key, entry => entry.cube);
        }

        tileCubes = tileCubesList.ToDictionary(entry => entry.key, entry => entry.cube);
        return tileCubes;
    }

    Dictionary<ETileType, List<GameObject>> doorsCubes = new Dictionary<ETileType, List<GameObject>>();

    void OnEnable()
    {
        EventBus.Subscribe<CrystalBreakEvent>(RemoveDoor);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<CrystalBreakEvent>(RemoveDoor);
    }


    void Start()
    {
        GetTileCubes();

        if (isTest)
            Test();
        else
            SetMapData(mapData);
    }

    public void Test()
    {
        PathFinder.Instance.mapData = mapData;
        PathFinder.Instance.Test();
        PathFinder.Instance.SetClosedList();
    }

    public void SetMapData(MapData _mapData)
    {
        mapData = _mapData;
        PathFinder.Instance.Init(_mapData);

        CreateMap();
    }

    public void CreateMap()
    {
        List<MapTile> bgTiles = mapData.bgTiles;
        List<MapTile> tiles = mapData.tiles;

        List<GameObject> blueDoors1 = new List<GameObject>();
        List<GameObject> blueDoors2 = new List<GameObject>();
        List<GameObject> blueDoors3 = new List<GameObject>();

        List<GameObject> redDoors1 = new List<GameObject>();
        List<GameObject> redDoors2 = new List<GameObject>();
        List<GameObject> redDoors3 = new List<GameObject>();

        doorsCubes.Clear();

        foreach (var item in bgTiles)
        {
            if (item.TileType == ETileType.None) continue;

            GameObject cube = Instantiate(tileCubes[item.TileType]);
            cube.transform.position = new Vector3(item.x, 0, item.z);
            cube.transform.parent = tileBGParent.transform;
        }

        foreach (var item in tiles)
        {
            if (item.TileType == ETileType.None) continue;
            if (item.TileType == ETileType.START)
            {
                player.transform.position = new Vector3(item.x, 1, item.z);
                player.SetTargetPosition(player.transform.position);
                continue;
            }

            GameObject cube = Instantiate(tileCubes[item.TileType]);
            cube.transform.position = new Vector3(item.x, 1, item.z);
            cube.transform.parent = tileParent.transform;

            if (item.TileType == ETileType.BlueDoor_1) blueDoors1.Add(cube);
            else if (item.TileType == ETileType.BlueDoor_2) blueDoors2.Add(cube);
            else if (item.TileType == ETileType.BlueDoor_3) blueDoors3.Add(cube);

            else if (item.TileType == ETileType.RedDoor_1) redDoors1.Add(cube);
            else if (item.TileType == ETileType.RedDoor_2) redDoors2.Add(cube);
            else if (item.TileType == ETileType.RedDoor_3) redDoors3.Add(cube);

            else if (item.TileType == ETileType.Moneter_1) spawner.GiveHpBar(cube);
            else if (item.TileType == ETileType.Moneter_2) spawner.GiveHpBar(cube);
            else if (item.TileType == ETileType.Monster_3) spawner.GiveHpBar(cube);

            //몬스터는 타일맵을 한겹더 구분하면 좋았을거같다.
        }

        doorsCubes.Add(ETileType.BlueCrystal_1, blueDoors1);
        doorsCubes.Add(ETileType.BlueCrystal_2, blueDoors2);
        doorsCubes.Add(ETileType.BlueCrystal_3, blueDoors3);

        doorsCubes.Add(ETileType.RedCrystal_1, redDoors1);
        doorsCubes.Add(ETileType.RedCrystal_2, redDoors2);
        doorsCubes.Add(ETileType.RedCrystal_3, redDoors3);
    }

    public void RemoveDoor(CrystalBreakEvent e)
    {
        List<GameObject> targets = doorsCubes[e.e];
        
        foreach (GameObject target in targets)
        {
            target.gameObject.SetActive(false);
            PathFinder.Instance.RemoveClostNode(target.gameObject.transform.position);
        }

        targets.Clear();
        doorsCubes[e.e].Clear();
    }
}
