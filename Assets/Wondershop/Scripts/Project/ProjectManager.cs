using UnityEngine;

/// <summary>
/// Game Project Manager
/// </summary>
/// <description>
/// This class controls all the required components needed for running the game including:
/// connecting to RePlay, loading the required Scenes and saving information as PlayerPref
/// </description>
/// 
public static class ProjectManager
{
	public static string MainScene => "Assets/MainScene.unity";

	private static void SetupSettings() {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Debug.unityLogger.logEnabled = true;
#else
        QualitySettings.vSyncCount = 0;
        Debug.unityLogger.logEnabled = false;
#endif
	}


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeInPlayer() => SetupSettings();
}