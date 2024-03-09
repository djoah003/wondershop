using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public abstract class GameState : MonoBehaviour
{
	[SerializeField] private GameStateConfig config;
	[SerializeField] private AudioClip music;
	[SerializeField] private GameObject transitionPrefab;

	private ITimed[] _timed = {
	};
	protected ITransition transition;
	private static UnityAction<bool> onStateComplete;
	protected bool EndGameEarly;

	public float timer { get; protected set; }

	/**
	 * Helpers
	 *
	 */
	private void OnTimerUpdate() {
		timer -= Time.deltaTime;
		OnTimed(timer);
	}

	private void OnTimerStops() {
		timer = 0;
		OnTimed(timer);
	}

	protected void OnTimed(float value) {
		foreach (ITimed timed in _timed) timed.SetTime(value);
	}

	private void OnTransitionEnter() {
		transition ??= Instantiate(transitionPrefab).GetComponent<ITransition>();
		DontDestroyOnLoad(transition.gameObject);
		transition.SetClosed();
	}

	private void OnTransitionExit() {
		transition ??= Instantiate(transitionPrefab).GetComponent<ITransition>();
		transition.SetOpen();
	}


	/**
	 * Unity life-cycle
	 *
	 */
	private void OnDisable() {
		onStateComplete = null;
		if ((Object)transition != null) Destroy(transition.gameObject);
		foreach (GameObject player in PlayerManager.Players()) OnPlayerExit(player);
	}

	/**
	 * Game state management
	 *
	 */
	public void OnStateEnter(UnityAction<bool> onCompleteCallback = null) {
		// start the music
		AudioManager.PlayMusic(music);
		// set initial callback
		onStateComplete = onCompleteCallback;
		// run through the players
		foreach (GameObject player in PlayerManager.Players()) OnPlayerEnter(player);
		// get all the timed components
		_timed = GetComponents<ITimed>();
		// check the transitions
		if (transitionPrefab) OnTransitionEnter();
		// proceed into the state
		OnEnter();
	}

	public void OnStateUpdate() {
		// auto exit when timer is zero
		if (timer < 0) OnStateExit();
		// update timer while over zero
		if (0 < timer) OnTimerUpdate();
		// call the state's own update
		OnUpdate();
	}

	public void OnStateExit() {
		OnTimerStops(); // zero out the timer
		OnBeforeExit(); // call onBeforeExit-hook
	}

	public void OnBeforeExitComplete() {
		if (transitionPrefab) OnTransitionExit(); // check the transitions
		Invoke(nameof(OnExit), transitionPrefab ? transition.Close() : 0f); // close out of the state
	}

	public void OnExitComplete() => OnAfterExit();

	public void OnStateComplete() {
		onStateComplete?.Invoke(EndGameEarly);
		foreach (GameObject player in PlayerManager.Players()) OnPlayerExit(player);
	}

	public void OnStateInterrupt() {
		OnTimerStops(); // zero out the timer
		OnExit(); // close state
	}

	/**
	 * Function to be overwritten
	 *
	 */
	protected virtual void OnEnter() {
	}

	protected virtual void OnUpdate() {
	}

	protected virtual void OnBeforeExit() => OnBeforeExitComplete();

	protected virtual void OnExit() => OnExitComplete();

	protected virtual void OnAfterExit() => OnStateComplete();


	/**
	 * Player state management
	 *
	 */
	protected virtual void OnPlayerEnter(GameObject player) {
	}

	protected virtual void OnPlayerExit(GameObject player) {
	}
}