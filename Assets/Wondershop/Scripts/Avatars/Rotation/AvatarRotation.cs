using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IRotation
{
	/// <summary>
	/// IMovement Move takes new movement input to start accruing the new
	/// movement target (move). Additionally, it needs to consider current
	/// velocity to perform the calculations.
	/// </summary>
	public void Rotate(Vector2 move, ref Quaternion rotationNext, ref Camera cam);
}


[RequireComponent(typeof(CharacterController))]
public class AvatarRotation : AvatarBehaviour, IAvatarPlugin
{
	private Vector2 _move;
	private ICamera _camera;
	private List<IRotation> _rotations = new List<IRotation>();
	private CharacterController _controller;

	/**
	 * Unity life-cycle
	 *
	 */
	private void OnEnable() {
		avatar.OnMove += Move;
		avatar.OnTick += UpdateRotation;
	}

	public void OnDisable() {
		avatar.OnMove -= Move;
		avatar.OnTick -= UpdateRotation;
	}

	/**
	 * IAvatarPlugin
	 *
	 */
	public void Register() {
		_camera = transform.parent.GetComponent<Avatar>().mainCamera;
		_rotations = GetComponentsInChildren<IRotation>().ToList();
		_controller = GetComponent<CharacterController>();
	}

	public void Unregister() => _rotations.Clear();


	/**
	 * Event listeners
	 *
	 */
	private void Move(Vector2 move) => _move = move;


	/**
	 * IRotation
	 *
	 */
	private void UpdateRotation() {
		// rotation input relies on camera transform 
		if (_camera == null || !_camera.TryGetCamera(out Camera cam) || !active) return;
		// rotation is relative to current rotation
		Quaternion prevRotation = transform.rotation;
		Quaternion nextRotation = prevRotation;
		// actually apply the rotations to character
		foreach (IRotation rotation in _rotations) rotation?.Rotate(_move, ref nextRotation, ref cam);
		_controller.transform.rotation = nextRotation;
	}
}