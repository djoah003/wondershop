using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public interface IInput
{
	public event Action push;
	public event Action<bool> hold;
	public event Action<Vector2> move;
}

/// <summary>
/// AvatarInput holds the state from the PlayerInput to provide a single point
/// of access for the Avatar class to react to the given input from controller
/// </summary>
public class AvatarInput : MonoBehaviour, IInput
{
	public event Action push;
	public event Action<bool> hold;
	public event Action<Vector2> move;
	private bool _hold;
	private bool _lock;


	/**
	 * State management
	 *
	 */
	private void PushInput(bool newPushState) {
		// TODO: ignore the lock for push for now to allow player welcome greeting
		// if (_lock) return;
		// the hold does not have release hook, so we fake it with the push release
		if (newPushState) push?.Invoke();
		else if (_hold) HoldInput(false);
	}

	private void HoldInput(bool newHoldState) {
		if (_lock) return;
		// track hold internally so we know to release it once the push is released
		_hold = newHoldState;
		hold?.Invoke(newHoldState);
	}

	private void MoveInput(Vector2 moveDirection) {
		if (_lock) return;
		move?.Invoke(moveDirection);
	}


	/**
	 * Event listeners
	 *
	 *
	 */
	public void OnPush(InputValue value) => PushInput(value.isPressed);
	public void OnHold(InputValue value) => HoldInput(value.isPressed);
	public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());


	/**
	 * Input coming from outside of InputSystem
	 *
	 */
	public void Push(bool value) => PushInput(value);
	public void Hold(bool value) => HoldInput(value);
	public void Move(Vector2 value) => MoveInput(value);

	/**
	 * External features
	 *
	 */
	public void Lock(bool locked) {
		PushInput(false);
		HoldInput(false);
		MoveInput(Vector2.zero);
		_lock = locked;
	}

	/// <summary>
	/// Helper function to lock or unlock all players
	/// </summary>
	/// <param name="locked"></param>
	public static void LockPlayers(bool locked) {
		List<GameObject> players = PlayerManager.Players();
		foreach (GameObject player in players) player.GetComponentInChildren<AvatarInput>().Lock(locked);
	}


}