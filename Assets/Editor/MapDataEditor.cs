#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
#endif

[CustomEditor(typeof(MakeMapData))]
public class MapDataEditor : Editor
{
    MakeMapData mapData;
    SerializedProperty makeMapData;
    SerializedProperty makeMapTile;
    SerializedProperty makeBgMapTile;
    SerializedProperty tileBGParent;
    SerializedProperty tileParent;

    void OnEnable()
    {
        mapData = target as MakeMapData;
        makeMapData = serializedObject.FindProperty("mapData");
        makeMapTile = serializedObject.FindProperty("tilemap");
        makeBgMapTile = serializedObject.FindProperty("bgTilemap");
        tileBGParent = serializedObject.FindProperty("tileBGParent");
        tileParent = serializedObject.FindProperty("tileParent");
    }

    public override void OnInspectorGUI()
    {
        #region Example

        //(MonsterType)EditorGUILayout.EnumPopup("", monster.monsterType);


        //EditorGUILayout.FloatField("", monster.hp);


        //EditorGUILayout.FloatField("", monster.atk);
        //EditorGUILayout.TextField("", monster.name);

        //EditorGUI.DrawPreviewTexture(Rect.zero, monster.sprite);
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("");

        //EditorGUILayout.ObjectField(monster.sprite, typeof(Sprite), false, new[] { GUILayout.Width(64), GUILayout.Height(64) }) as Sprite;
        //monster.sprite = EditorGUILayout.ObjectField(monster.sprite, typeof(Sprite), false) as Sprite;

        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.Slider("value", monster.value, 0f, 1f);


        #endregion

        mapData.startPos = EditorGUILayout.Vector2IntField("StartPos", mapData.startPos);
        mapData.endPos = EditorGUILayout.Vector2IntField("EndPos", mapData.endPos);

        serializedObject.Update();

        EditorGUILayout.PropertyField(makeMapData);

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(makeBgMapTile);
        EditorGUILayout.PropertyField(makeMapTile);

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(tileParent);
        EditorGUILayout.PropertyField(tileBGParent);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Tile Sprites", EditorStyles.boldLabel);

        for (int i = 0; i < mapData.tileSpritesList.Count; i++)
        {
            var entry = mapData.tileSpritesList[i];

            EditorGUILayout.BeginHorizontal();

            // Enum dropdown
            entry.key = (ETileType)EditorGUILayout.EnumPopup(entry.key, GUILayout.Width(150));
            entry.sprite = (Sprite)EditorGUILayout.ObjectField(entry.sprite, typeof(Sprite), false);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                mapData.tileSpritesList.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Tile Sprite"))
        {
            mapData.tileSpritesList.Add(new TileSpriteEntry());
        }

        mapData.GetTileSprites();

        GUILayout.Space(30);




        GUILayout.Space(20);
        EditorGUILayout.LabelField("Tile Cubes", EditorStyles.boldLabel);

        for (int i = 0; i < mapData.tileCubesList.Count; i++)
        {
            var entry = mapData.tileCubesList[i];

            EditorGUILayout.BeginHorizontal();

            // Enum dropdown
            entry.key = (ETileType)EditorGUILayout.EnumPopup(entry.key, GUILayout.Width(150));
            entry.cube = (GameObject)EditorGUILayout.ObjectField(entry.cube, typeof(GameObject), false);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                mapData.tileCubesList.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Tile Cube"))
        {
            mapData.tileCubesList.Add(new TileCubeEntry());
        }

        GUILayout.Space(30);

        mapData.GetTileCubes();

        serializedObject.ApplyModifiedProperties();



        if (GUILayout.Button("Save"))
        {
            Debug.Log("Save");
            mapData.Save();
        }

        if (GUILayout.Button("Load"))
        {
            Debug.Log("Load");
            mapData.LoadMap();
        }

        if (GUILayout.Button("Load TileMap"))
        {
            Debug.Log("Load");
            mapData.LoadTileMap();
        }


        GUILayout.Space(10f);

        if (GUILayout.Button("Save Map Data"))
        {
            SampleWindow s = new SampleWindow();
            s.makeMapData = mapData;
            s.OpenSaveWindow();
        }

        if (GUILayout.Button("Load Map Data"))
        {
            SampleWindow s = new SampleWindow();
            s.makeMapData = mapData;
            s.OpenLoadWindow();
        }

        GUILayout.Space(10f);

        if (GUILayout.Button("Clear Data"))
        {
            Debug.Log("Clear Data");
            mapData.ClearData();
        }

        if (GUILayout.Button("Clear Map"))
        {
            Debug.Log("Clear Map");
            mapData.ClearMap();
        }

        if (GUILayout.Button("Clear TileMap"))
        {
            Debug.Log("Clear Map");
            mapData.ClearTileMap();
        }
    }
}