using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SampleWindow : EditorWindow
{
    public MakeMapData makeMapData;

    private MapData[] mapDatas;
    private bool isSave;

    private void OnGUI()
    {
        mapDatas = Resources.LoadAll<MapData>("MapData");

        for (int i = 0; i < mapDatas.Length; i++)
        {
            if (GUILayout.Button(mapDatas[i].name))
            {
                makeMapData.mapData = mapDatas[i];

                if (isSave)
                    makeMapData.Save();
                else
                    makeMapData.LoadMap();
            }
        }
    }

    public void OpenLoadWindow()
    {
        isSave = false;

        SampleWindow window = (SampleWindow)EditorWindow.GetWindow(typeof(SampleWindow), false, "LoadMapDataList");
        window.title = "Load Map Data List";
        window.Show();
    }

    public void OpenSaveWindow()
    {
        isSave = true;

        SampleWindow window = (SampleWindow)EditorWindow.GetWindow(typeof(SampleWindow), false, "Save Map Data List");
        window.title = "Save Map Data List";
        window.Show();
    }

    [MenuItem("Tools/Save")]
    static void Save()
    {

    }
    [MenuItem("Tools/Load")]
    static void Load()
    {

    }

}