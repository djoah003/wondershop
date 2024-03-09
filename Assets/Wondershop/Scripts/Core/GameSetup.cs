using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
	[SerializeField] private GameSetupConfig config;
	[Header("Events")] [SerializeField] private ScriptableEventForGameObject onPlayerGameStart;
	[SerializeField] private ScriptableEventForGameObject onPlayerGameEnd;

	private int _loopIndex;

	/**
	 * Unity life-cycle
	 *
	 */
	private void Start() {
		foreach (GameObject player in PlayerManager.Players()) OnPlayerJoined(player);
		NextStage();
	}

	/**
	 * Go trough the stage loop
	 *
	 */
	private void NextStage() {
		AsyncOperation sceneLoad =
			SceneManager.LoadSceneAsync(config.stages[_loopIndex].scenePath, LoadSceneMode.Additive);
		sceneLoad.completed += _ => FindObjectOfType<GameManager>().OnStageEnter(ExitStage);
	}

	private void ExitStage() {
		SceneManager.UnloadSceneAsync(config.stages[_loopIndex].scenePath);
		_loopIndex++;
		if (_loopIndex < config.stages.Length) NextStage();
		else MainManager.Instance.OnGameSetupComplete();
	}


	/**
	 * Event listeners
	 *
	 */
	[EventListener]
	public void OnPlayerJoined(GameObject player) {
		if (onPlayerGameStart) onPlayerGameStart.TriggerEvent(player);
	}

	[EventListener]
	public void OnPlayerLeft(GameObject player) {
		if (onPlayerGameEnd) onPlayerGameEnd.TriggerEvent(player);
	}
}