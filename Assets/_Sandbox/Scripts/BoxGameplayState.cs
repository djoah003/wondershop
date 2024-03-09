using Cinemachine;
using UnityEngine;

/***
 * You gamestate logic goes here. There are events coming from the ScriptableEvents and the
 * gamestate decides the "rules" on how those events effect other objects in the scene.
 *
 */
public class DemoGameplayState : GameState
{
	// playerBehavior is the collection of components that have their behaviour injected to 
	// new players upon joining. This allows a "plugin" system for avatar, but can be a bit complex.
	// If for some reason it causes more gray here than its worth, you can optionally just create a
	// complete player prefab
	[SerializeField] private GameObject playerBehavior;
	private ILevels _levels;
	private CinemachineTargetGroup _cameraTargetGroup;

	/**
	 * Helpers
	 *
	 */
	// example of some game specific helpers
	private static void LockPlayer(GameObject avatar, bool value = true) {
		foreach (AvatarBehaviour behaviour in avatar.GetComponents<AvatarBehaviour>()) behaviour.SetActive(!value);
	}


	/**
	 * Custom life-cycle
	 *
	 */
	// this is called by our custom GameManager upon loading the scene
	protected override void OnEnter() {
		// load the level
		_levels ??= GetComponent<LevelsSingles>();
		if (_levels != null) _levels.CreateLevel(OnLevelCreated);
		else OnLevelCreated();
	}

	// called after the onEnter is finished
	private void OnLevelCreated() {
		// get the target group camera
		_cameraTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
		// trigger the transitions
		transition?.Open();
	}

	// this should be called when you want the game to end and load next level
	protected override void OnExit() {
		// remove the current level and notify that state in complete
		if (!_levels.Equals(null)) _levels.RemoveLevel(OnStateComplete);
		else OnStateComplete();
	}

	protected override void OnPlayerEnter(GameObject player) {
		player.GetComponent<Avatar>().Inject(playerBehavior);
		_cameraTargetGroup.AddMember(player.transform.GetChild(0), 1f, 1f);
	}


	protected override void OnPlayerExit(GameObject player) {
		player.GetComponent<Avatar>().Deject(playerBehavior);
		_cameraTargetGroup.RemoveMember(player.transform.GetChild(0));
	}


	/**
	 * Event listeners
	 *
	 */
	[EventListener]
	public void OnPlayerJoined(GameObject player) {
		ProjectLogger.Log($"[{GetType().Name}] OnPlayerJoined");
		OnPlayerEnter(player);
	}

	[EventListener]
	public void OnPlayerLeft(GameObject player) {
		ProjectLogger.Log($"[{GetType().Name}] OnPlayerLeft");
	}

	/**
	 * Collisions
	 *
	 */
	[EventListener]
	public void OnPlayerCollision(ColliderEventArgs colliderEvent) {
		// NOTE: this is called based on the AvatarCollision components layer setup, if layer is everything it gets spammed all the time
		// make sure to set it to the layer that you are actually interested in for example items etc.
		// ProjectLogger.Log($"[{colliderEvent.That}] OnPlayerCollision");
	}

	/**
	 * Triggers
	 *
	 */
	[EventListener]
	public void OnPlayerTriggerEnter(ColliderEventArgs colliderEvent) {
		ProjectLogger.Log($"[{colliderEvent.That}] OnPlayerTriggerEnter");
	}

	[EventListener]
	public void OnPlayerTriggerExit(ColliderEventArgs colliderEvent) {
		ProjectLogger.Log($"[{colliderEvent.That}] OnPlayerTriggerExit");
	}

	/**
	 * Interactions
	 *
	 */
	[EventListener]
	public void OnPlayerInteractionEnter(ColliderEventArgs interaction) {
		interaction.Other.GetComponentInChildren<IInteracting>().EnterInteraction(interaction.That);
		interaction.That.GetComponent<IHighlight>()?.Focus();
	}

	[EventListener]
	public void OnPlayerInteractionStart(ColliderEventArgs interaction) {
		ProjectLogger.Log($"[{interaction.That}] OnPlayerInteractionStart");
		GameObject avatar = interaction.That;
	}

	[EventListener]
	public void OnPlayerInteractionEnd(ColliderEventArgs interaction) {
		ProjectLogger.Log($"[{interaction.That}] OnPlayerInteractionEnd");
		GameObject avatar = interaction.That;
	}

	[EventListener]
	public void OnPlayerInteractionExit(ColliderEventArgs interaction) {
		interaction.Other.GetComponentInChildren<IInteracting>().ExitInteraction(interaction.That);
		interaction.That.GetComponent<IHighlight>()?.Unfocus();
	}

	/**
	 * Actions
	 *
	 */
	[EventListener]
	public void OnPlayerActionPress(GameObject player) {
		// ProjectLogger.Log($"[{GetType().Name}] OnPlayerActionPush");
	}

	[EventListener]
	public void OnPlayerActionHold(GameObject player) {
		// Currently the hold is very short time, 0.4seconds so this is almost the same 
		// as press, you can either tweak the input settings or figure out other ways to detect hold
		//ProjectLogger.Log($"[{GetType().Name}] OnPlayerActionHold");
	}
}