using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// SetupManager handles scene configuration and logic between games and lobby
/// </summary>
/// <description>
/// Loads the scene configuration and rotates scenes based on that configuration.
/// </description>
public class MainManager : ProjectSingleton<MainManager>
{
	[SerializeField] private MainConfig config;
	[SerializeField] private VideoClip video;
	private bool _wait;
	private bool _main;
	private int _index;
	private Scene _currentScene;

	/**
	 * Editor flag
	 *
	 */
	public static bool singleGameMode;

	/**
	 * Unity life-cycle
	 *
	 */
	protected override void Awake() {
		base.Awake();
#if UNITY_EDITOR
		// in editor, to allow testing single scene without lobby, we can use the value provided by the hook
		string initialScene = EditorPrefs.GetString("EditorScene", string.Empty);
		if (!EditorPrefs.GetBool("WondershopSettings.PlayStartSequence")) video = null;
#else
        string initialScene = SceneManager.GetActiveScene().name;
#endif
		bool isSetupScene = initialScene == System.IO.Path.GetFileNameWithoutExtension(ProjectManager.MainScene);
		if (!isSetupScene) StartSingleGame(initialScene);
		else StartMain();
	}

	/**
	 * Scene management
	 *
	 */
	private IEnumerator LoadSceneByName(string sceneName) {
		ProjectLogger.Log($"LoadSceneByName {sceneName}");
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		_wait = !asyncLoad.isDone;
		while (!asyncLoad.isDone) yield return null;
		_wait = !asyncLoad.isDone;
		ActivateScene(SceneManager.GetSceneByName(sceneName));
	}

	private IEnumerator LoadSceneByPath(string scenePath) {
		ProjectLogger.Log($"LoadSceneByPath {scenePath}");
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
		_wait = !asyncLoad.isDone;
		while (!asyncLoad.isDone) yield return null;
		_wait = !asyncLoad.isDone;
		ActivateScene(SceneManager.GetSceneByPath(scenePath));
	}

	private void ActivateScene(Scene scene) {
		SceneManager.SetActiveScene(scene);
		_currentScene = scene;
	}

	private static void UnloadScene(string sceneName) {
		ProjectLogger.Log($"Unloading {sceneName}");
		SceneManager.UnloadSceneAsync(sceneName);
		Resources.UnloadUnusedAssets();
	}

	/**
	 * State management
	 *
	 */
	private void StartSingleGame(string sceneName) {
		singleGameMode = true;
		StartCoroutine(LoadSceneByName(sceneName));
	}

	private void StartMain() {
		if (video != null) StartVideo();
		else StartGame();
	}

	private void StartVideo() {
		GameObject intro = new GameObject(name: "IntroVideoPlayer");
		Camera videoCamera = intro.AddComponent<Camera>();
		videoCamera.clearFlags = CameraClearFlags.Color;
		videoCamera.backgroundColor = Color.black;
		VideoPlayer videoPlayer = intro.AddComponent<VideoPlayer>();
		videoPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
		videoPlayer.playOnAwake = false;
		videoPlayer.waitForFirstFrame = true;
		videoPlayer.clip = video;
		videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
		videoPlayer.prepareCompleted += source => source.Play();
		videoPlayer.loopPointReached += StopVideo;
		videoPlayer.Prepare();
	}

	private void StopVideo(VideoPlayer videoPlayer) {
		Destroy(videoPlayer.gameObject);
		StartGame();
	}

	private void StartGame() {
		_main = true;
		singleGameMode = false;
		string scenePath = config.setup != null ? config.setup.scenePath : "";
		// try to get the lobby or the video scene, if neither available start first game
		if (!string.IsNullOrEmpty(scenePath)) StartCoroutine(LoadSceneByPath(scenePath));
		else StartSingleGame(System.IO.Path.GetFileNameWithoutExtension(config.games[0].scenePath));
	}

	private void NextSetup() {
		_main = !_main;
		if (_main) StartCoroutine(LoadSceneByPath(config.setup.scenePath));
		else NextGame();
	}

	private void NextGame() {
		if (config.games.Length <= _index) _index = 0;
		StartCoroutine(LoadSceneByPath(config.games[_index].scenePath));
		_index++;
	}

	/**
	 * Public API
	 *
	 */
	public void OnGameSetupComplete() {
		AudioManager.StopMusic();
		PointManager.Instance.ClearPoints();
		PointManager.Instance.ClearScores();
		TeamManager.Instance.TeamsReset();
		UnloadScene(_currentScene.name);
		NextSetup();
	}


	/**
	 * Debug
	 *
	 */
	public void DebugNextScene() {
		if (_wait || singleGameMode) return;
		GameManager gameManager = FindObjectOfType<GameManager>();
		if (gameManager.Equals(null)) OnGameSetupComplete();
		else gameManager.DebugNextScene();
	}
}