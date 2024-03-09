using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/**
 * GameLoading is focused on loading the specific game states, that are currently scene based.
 * Once the scene has been loaded, it will return the new Scene's GameState object for Manager.
 *
 */
public static class GameLoading
{
	private static void ActiveGame(string scene) {
		try {
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(System.IO.Path.GetFileNameWithoutExtension(scene)));
		} catch (ArgumentException) {
			// ignored
		}
	}


	public static void LoadGame(string scene, UnityAction<GameState> callback) {
		AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
		sceneLoad.completed += _ => callback?.Invoke(Object.FindObjectOfType<GameState>());
		sceneLoad.completed += _ => ActiveGame(scene);
	}

	public static void UnloadGame(string scene, UnityAction callback) {
		AsyncOperation sceneUnload = SceneManager.UnloadSceneAsync(scene);
		if (callback != null) sceneUnload.completed += _ => callback.Invoke();
	}
}