using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/**
 * GameManager handles the state management when toggling between the
 * multiple states, it will retain required information and functionality.
 *
 */
public class GameManager : MonoBehaviour
{
	[SerializeField] private GameStageConfig config;

	private GameState _currentState;
	private string _currentScene;

	public static int currentRound;
	private static int currentTotal;
	private static int currentIndex;
	private static UnityAction onStageComplete;

	private bool hasValidScenes => config.states != null && config.states.Length != 0;
	public static bool IsLastRound() => currentRound == currentTotal - 1;
	public static bool IsFirstRound() => currentRound == 0;

	/**
	 * Unity life-cycle
	 *
	 */
	private void Update() => OnStateUpdate();

	private void OnDisable() {
		if (_currentState) GameLoading.UnloadGame(_currentScene, null);
	}

	/**
	 * Game state management
	 *
	 */
	public void OnStageEnter(UnityAction onStageCompleteCallback) {
		onStageComplete = onStageCompleteCallback;
		currentIndex = 0;
		currentRound = currentIndex;
		currentTotal = config.numberOfRounds;
		ChangeState();
	}

	private void ChangeState() {
		_currentScene = config.states[currentIndex].scenePath;
		if (hasValidScenes) GameLoading.LoadGame(_currentScene, OnStateEnter);
	}

	private void OnStateEnter(GameState gameState) {
		_currentState = gameState;
		_currentState.OnStateEnter(OnStateComplete);
	}

	private void OnStateUpdate() {
		if (_currentState) _currentState.OnStateUpdate();
	}

	private void OnStateExit() {
		if (_currentState) _currentState.OnStateExit();
	}

	private void OnStateComplete(bool endGameEarly = false) {
		if (endGameEarly) {
			onStageComplete();
			return;
		}
		if (!_currentScene.Equals(string.Empty)) GameLoading.UnloadGame(_currentScene, NextState);
		else NextState();
	}


	/**
	 * Game round management
	 *
	 */
	private void NextState() {
		_currentState = null;
		currentIndex += 1;
		if (config.states.Length - 1 < currentIndex) ResetStage();
		else ChangeState();
	}

	private void ResetStage() {
		currentIndex = 0;
		if (ShouldEnd() && onStageComplete != null) onStageComplete();
		else NextRound();
	}

	private static bool ShouldEnd() {
		if (MainManager.singleGameMode) return false;
		return currentTotal - 1 <= currentRound;
	}

	private void NextRound() {
		currentRound++;
		ChangeState();
	}

	/**
	 * Common features
	 *
	 */
	public static void OnPlayerPoint(GameObject player, PointConfig config, int points = 1, bool incremental = false) {
		PointManager.Instance.AddPoints(player.GetComponent<PlayerInput>().playerIndex, points,
			config, currentRound);
	}

	/**
	 * Debug features
	 *
	 */
	public void DebugNextScene() {
		currentTotal = currentRound + 1;
		if (_currentState) _currentState.OnStateInterrupt();
		currentRound = currentTotal;
	}
}