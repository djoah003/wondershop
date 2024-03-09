using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerAction
{
	Push,
	Hold,
	Move,
}

public interface IAction
{
	public void ActionInput(Vector2 move, bool press, bool hold);
}

public class AvatarAction : AvatarBehaviour, IAvatarPlugin
{
	private bool _push;
	private bool _hold;
	private Vector2 _move;
	private List<IAction> _actions = new List<IAction>();

	/**
	 * Unity life-cycle
	 *
	 */
	public void OnEnable() {
		avatar.OnMove += Move;
		avatar.OnPush += Push;
		avatar.OnHold += Hold;
		avatar.OnTick += UpdateAction;
	}

	public void OnDisable() {
		avatar.OnMove -= Move;
		avatar.OnPush -= Push;
		avatar.OnHold -= Hold;
		avatar.OnTick -= UpdateAction;
	}

	/**
	 * Avatar Behaviour
	 *
	 */
	public override void SetActive(bool value) {
		// NOTE: Actions are special case where we allow them to be active always
	}


	/**
	 * IAvatarPlugin
	 *
	 */
	public void Register() => _actions = GetComponentsInChildren<IAction>().ToList();

	public void Unregister() => _actions.Clear();


	/**
	 * Event listeners
	 *
	 */
	private void Push() => _push = true;
	private void Hold(bool value) => _hold = value;
	private void Move(Vector2 move) => _move = move;

	/**
	 * Actual Behavior
	 */
	private void UpdateAction() {
		foreach (IAction action in _actions) action.ActionInput(_move, _push, _hold);
		_push = false;
	}
}