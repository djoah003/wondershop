using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;


[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : ProjectSingleton<PlayerManager>
{
	public const int MaxPlayerCount = 8;
	[SerializeField] private GameObject defaultPlayerPrefab;
	[Header("Events")] [SerializeField] private ScriptableEventForGameObject onPlayerJoinedEvent;
	[SerializeField] private ScriptableEventForGameObject onPlayerLeftEvent;
	[SerializeField] private ScriptableEventForGameObject onPlayerControllerBattery;

	/**
	 * Unity life-cycle
	 *
	 */
	private void Start() {
		PlayerInputManager.instance.EnableJoining();
		PlayerInputManager.instance.playerPrefab = defaultPlayerPrefab;
		TogglePlayerJoining();
	}

	/**
	 * Player(s) management
	 *
	 */
	private static void TogglePlayerJoining() {
		if (Players(ignoreBots: true).Count < MaxPlayerCount) PlayerInputManager.instance.EnableJoining();
		else PlayerInputManager.instance.DisableJoining();
	}


	/**
	 * Event handlers
	 *
	 */
	[UsedImplicitly]
	private void OnPlayerJoined(PlayerInput playerInput) {
		// if player uses Virtual controlSchema, they are bot
		// and should have their current input disabled
		if (playerInput.currentControlScheme == "Virtual") playerInput.DeactivateInput();
		// call custom "Awake" i.e. Setup function for Player
		playerInput.gameObject.GetComponent<IAvatar>().Setup(() => onPlayerJoinedEvent.TriggerEvent(playerInput.gameObject));
		// set players as "DontDestroyOnLoad"
		DontDestroyOnLoad(playerInput.gameObject);
		// check if player should be allowed to join
		TogglePlayerJoining();
	}

	[UsedImplicitly]
	private void OnPlayerLeft(PlayerInput playerInput) {
		ProjectLogger.Log($"OnPlayerLeft: " + playerInput.playerIndex);
		// trigger the OnPlayerLeft
		onPlayerLeftEvent.TriggerEvent(playerInput.gameObject);
		// check if player should be allowed to join
		TogglePlayerJoining();
	}

	[UsedImplicitly]
	public void OnPlayerControllerRemoved(string serial) {
		// find player with the serial
		GameObject player = Players().First(player => player.GetComponent<IAvatar>().Model.Player().ID == serial);
		// if player is found, despawn it
		if (!player.Equals(null)) Destroy(player);
	}


	/**
	 * Players interface
	 *
	 */
	public static bool IsBot(PlayerInput playerInput) => !playerInput.inputIsActive;

	public static List<PlayerInput> PlayerInputs(bool ignoreBots = false) => ignoreBots
		? PlayerInput.all.Where(input => !IsBot(input)).ToList()
		: PlayerInput.all.ToList();

	public static List<GameObject> Players(bool ignoreBots = false) =>
		PlayerInputs(ignoreBots).Select(input => input.gameObject).ToList();
}