using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene auto setup
/// </summary>
/// <description>
/// This class will update the currently open scene so that SetupScene will load it if needed
/// </description>
///
[InitializeOnLoad]
public static class SceneAutoSetup
{
    // Static constructor binds a playmode-changed callback.
    // [InitializeOnLoad] above makes sure this gets executed.
    static SceneAutoSetup()
    {
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectManager.MainScene);
        EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
    }

    private static void OnSceneChanged(Scene prev, Scene next) => EditorScene = next.name;
    private const string CEditorPrefEditorScene = "EditorScene";

    private static string editorScene;

    public static string EditorScene
    {
        get => EditorPrefs.GetString(CEditorPrefEditorScene, editorScene);
        set
        {
            editorScene = value;
            EditorPrefs.SetString(CEditorPrefEditorScene, value);
        }
    }
}