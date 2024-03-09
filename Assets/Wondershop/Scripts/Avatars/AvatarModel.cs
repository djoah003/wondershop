using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IModel
{
	public void Sync(PlayerInput playerInput, Action onSetupDone);
	public int Index();
	public PlayerModel Player();
	public ControllerModel Controller();
}

/// <summary>
/// Handler for syncing data with the IStorage
/// NOTE: this is now a cleaned up version without backend support, normally there
/// would be async tasks fetching player data, but for sandbox we do not need that
/// </summary>
public class AvatarModel : MonoBehaviour, IModel
{
	private int _index;
	private bool _offline;
	private Action _onSetupDone;
	private PlayTagInputDevice _playTag;

	/**
	 * Custom life-cycle
	 *
	 */
	public void Sync(PlayerInput playerInput, Action onSetupDone) {
		_index = playerInput.playerIndex + 1;
		onSetupDone?.Invoke();
	}

	public int Index() => _index;
	public PlayerModel Player() => _playerModel;
	public ControllerModel Controller() => _controllerModel;
	private readonly ControllerModel _controllerModel = new ControllerModel();
	private readonly PlayerModel _playerModel = new PlayerModel();
}