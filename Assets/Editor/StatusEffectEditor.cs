using Codice.Client.BaseCommands.Merge.Xml;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatusEffectData))]
public class StatusEffectEditor : Editor
{
    StatusEffectData effectData;

    SerializedProperty statusType;
    SerializedProperty statusName;
    SerializedProperty statusDuration;

    SerializedProperty statusValueCount;
    SerializedProperty statusValues;


    void OnEnable()
    {
        effectData = target as StatusEffectData;

        statusType = serializedObject.FindProperty("eStatusEffect");
        statusName = serializedObject.FindProperty("effectName");
        statusDuration = serializedObject.FindProperty("duration");

        statusValueCount = serializedObject.FindProperty("valueArraySize");
        statusValues = serializedObject.FindProperty("values");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(statusType);
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(statusName);
        EditorGUILayout.PropertyField(statusDuration);
        GUILayout.Space(10);
        statusValueCount.intValue = Mathf.Max
            (1, EditorGUILayout.IntField("Values Array Size", statusValueCount.intValue));

        EStatusEffect itemType = (EStatusEffect)statusType.enumValueIndex;
        statusValues.arraySize = (int)statusValueCount.intValue;

        EditorGUILayout.PropertyField(statusValues, true);
        serializedObject.ApplyModifiedProperties();
    }
}
