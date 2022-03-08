using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingsController))]
public class SettingsEditor : Editor
{
    SerializedProperty settings;

    private void OnEnable()
    {
        settings = serializedObject.FindProperty("currentSettings");
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();      

        SettingsController myScript = (SettingsController)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(settings);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(20);

        if (GUILayout.Button("open settings file", GUILayout.Height(50)))
        {
            myScript.OpenFile();
        }

        EditorGUILayout.Space(20);

        if (GUILayout.Button("save settings file", GUILayout.Height(50)))
        {
            myScript.SaveFile();
        }
    }
}
