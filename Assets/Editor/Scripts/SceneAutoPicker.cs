using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneAttribute))]
public class SceneAutoPicker : PropertyDrawer
{
    private static void SelectScene(SerializedProperty property, string stringValue)
    {
        property.stringValue = stringValue;
        // check if scene is already in EditorBuildSettings
        if (SceneUtility.GetBuildIndexByScenePath(stringValue) != -1) return;
        // if not, include it
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = EditorBuildSettings.scenes.ToList();
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(stringValue, true));
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent guiContent)
    {
        SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
        EditorGUI.BeginChangeCheck();
        SceneAsset newScene =
            EditorGUI.ObjectField(position, property.displayName, oldScene, typeof(SceneAsset), false) as SceneAsset;
        if (EditorGUI.EndChangeCheck()) SelectScene(property, AssetDatabase.GetAssetPath(newScene));
    }
}