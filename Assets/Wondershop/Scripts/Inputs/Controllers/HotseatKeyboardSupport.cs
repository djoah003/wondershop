using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class HotseatKeyboardSupport : MonoBehaviour
{
	private int _currentPlayerIndex;

	public void Update() {
		// null check the Keyboard
		if (Keyboard.current == null) return;
		// handle toggling
		if (Keyboard.current.tabKey.wasPressedThisFrame) {
			ProjectLogger.Log("Focus on next player with [Keyboard.tabKey]");
			OnEditorToggle();
		}

		// handle despawning
		if (Keyboard.current.backspaceKey.wasPressedThisFrame) {
			ProjectLogger.Log("Despawn Player Manually From [Keyboard.backspace]");
			Despawn();
		}

		// handle spawning
		if (!Keyboard.current.enterKey.wasPressedThisFrame) return;

		// limit spawning when max player count is reached
		if (!PlayerInputManager.instance.joiningEnabled) {
			ProjectLogger.Log("Max player count reached");
			return;
		}

		ProjectLogger.Log("Spawn Player Manually From [Keyboard.enterKey]");
		Spawn();
	}

	// Spawn new player
	private void Spawn() {
		try {
			PlayerInput player = GetComponent<PlayerInputManager>()
				.JoinPlayer(controlScheme: "KeyboardLeft", pairWithDevice: Keyboard.current);
			if (player == null) return;
			player.SwitchCurrentActionMap("Player");
			OnEditorToggle();
		} catch (ArgumentOutOfRangeException) {
			ProjectLogger.LogWarning("Toggling players too fast, slow down!");
		}
	}

	// Despawn existing players
	private void Despawn() {
		try {
			List<PlayerInput> players = PlayerManager.PlayerInputs(ignoreBots: true);
			if (players.Count == 0) return;
			// get the current player by index
			PlayerInput player = players[_currentPlayerIndex];
			Destroy(player.gameObject);
			StartCoroutine(WaitForPlayerLeft(player.gameObject));
		} catch (ArgumentOutOfRangeException) {
			ProjectLogger.LogWarning("Despawning players too fast, slow down!");
		}
	}

	private IEnumerator WaitForPlayerLeft(Object player) {
		yield return new WaitWhile(() => player != null);
		OnEditorToggle();
	}

	// Toggle through the controlled players
	private void OnEditorToggle() {
		List<PlayerInput> players = PlayerManager.PlayerInputs(ignoreBots: true);
		// nothing to toggle through
		if (players.Count == 0) return;
		// unpair all of the devices
		foreach (PlayerInput playerInput in players) playerInput.user.UnpairDevices();
		// switch control of previous player
		ProjectLogger.Log("Prev Player Index Is " + _currentPlayerIndex);
		_currentPlayerIndex = players.Count - 1 < _currentPlayerIndex ? players.Count - 1 : _currentPlayerIndex;
		PlayerInput prevPlayer = players[_currentPlayerIndex];
		prevPlayer.SwitchCurrentControlScheme("KeyboardRight", Keyboard.current);
		prevPlayer.SwitchCurrentActionMap("Player");
		// switch control of the next player
		_currentPlayerIndex = _currentPlayerIndex < players.Count - 1 ? _currentPlayerIndex + 1 : 0;
		ProjectLogger.Log("Next Player Index Is " + _currentPlayerIndex);
		PlayerInput nextPlayer = players[_currentPlayerIndex];
		nextPlayer.SwitchCurrentControlScheme("KeyboardLeft", Keyboard.current);
		nextPlayer.SwitchCurrentActionMap("Player");
	}
}