using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// IMovement only cares about moving the CharacterController for the next frame
/// </summary>
public interface IMovement
{
	public void Move(Vector2 move, bool press, bool hold, ref Vector3 moveNext, ref CharacterController controller,
		ref Camera cam);
}

/// <summary>
/// ISpeed can be used to modify movement settings, if supported by movement
/// </summary>
public interface ISpeed
{
	public float GetSpeed(); //TODO: set speed until
	public void SetSpeed(float speed);
	public void SetSpeedMultiplier(float multiplier = 1.0f, bool force = false);
}

[RequireComponent(typeof(CharacterController))]
public class AvatarMovement : AvatarBehaviour, IAvatarPlugin
{
	private bool _push;
	private bool _hold;
	private Vector2 _move;

	private ICamera _camera;
	private List<IMovement> _movements = new List<IMovement>();
	private CharacterController _controller;

	//TODO: Fixes charactercontroller not having velocity
	public Vector2 GetMove() => _move;

	/**
	 * Unity life-cycle
	 *
	 */
	public void OnEnable() {
		avatar.OnMove += Move;
		avatar.OnPush += Push;
		avatar.OnHold += Hold;
		avatar.OnTick += UpdateMovement;
	}

	public void OnDisable() {
		avatar.OnMove -= Move;
		avatar.OnPush -= Push;
		avatar.OnHold -= Hold;
		avatar.OnTick -= UpdateMovement;
	}


	/**
	 * Helpers
	 *
	 */
	public static bool Grounded(CharacterController cc, int groundLayers, float offset = 0.1f,
		bool detectIgnoreCollision = true) {
		// set sphere position, with offset
		Vector3 position = cc.transform.position;
		Vector3 spherePosition = new(position.x, position.y - offset, position.z);
		// custom check to allow modification
		bool customCheck = Physics.CheckSphere(spherePosition, cc.radius, groundLayers, QueryTriggerInteraction.Ignore);
		// custom solution does not take into account if character controller ignores some collisions
		bool controllerCheck = !detectIgnoreCollision || cc.isGrounded;
		// compound grounded check
		return customCheck && controllerCheck;
	}

	/**
	 * IAvatarPlugin
	 *
	 */
	public void Register() {
		_camera = avatar.mainCamera;
		_controller = GetComponent<CharacterController>();
		_movements = GetComponentsInChildren<IMovement>().ToList();
	}

	public void Unregister() => _movements.Clear();


	/**
	 * Event listeners
	 *
	 */
	private void Push() => _push = true;
	private void Hold(bool value) => _hold = value;
	private void Move(Vector2 move) => _move = move;


	/**
	 * IMovement
	 *
	 */
	private void UpdateMovement() {
		// movement depends on the camera transform
		if (_camera == null || !_camera.TryGetCamera(out Camera cam) || !active) return;
		Vector3 nextMove = Vector3.zero;
		foreach (IMovement movement in _movements)
			movement.Move(_move, _push, _hold, ref nextMove, ref _controller, ref cam);
		// actually move the character via controller if input
		if (!nextMove.Equals(Vector3.zero)) _controller.Move(nextMove);
		// _push will be cleared after it has been processed
		_push = false;
	}
}