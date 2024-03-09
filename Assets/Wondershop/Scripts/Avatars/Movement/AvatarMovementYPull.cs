using UnityEngine;

public class AvatarMovementYPull : AvatarBehaviour, IMovement
{
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float gravity = -9.81f;

	[Tooltip("What layers the character uses as ground")]
	public LayerMask groundLayers;

	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float fallTimeout = 0.15f;

	private float _fallTimeoutDelta;
	private float _verticalVelocity;
	private const float TerminalVelocity = -33.0f;


	/**
	 * Helpers
	 *
	 */
	private void OnGround() {
		// always reset the fall timer on ground
		_fallTimeoutDelta = fallTimeout;
		// do not increase velocity when grounded
		if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
	}

	private void OnAir() {
		// decrement fall timeout timer whe on air 
		if (0 <= _fallTimeoutDelta) _fallTimeoutDelta -= Time.deltaTime;
	}


	/**
	 * IMovement
	 *
	 */
	public void Move(Vector2 move, bool press, bool hold, ref Vector3 moveNext, ref CharacterController controller,
		ref Camera cam) {
		if (!active) return;
		bool grounded = AvatarMovement.Grounded(controller, groundLayers);
		if (grounded) OnGround();
		else OnAir();
		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (TerminalVelocity < _verticalVelocity) _verticalVelocity += gravity*Time.deltaTime;
		// apply the gravity to the Y axis for movement
		moveNext += new Vector3(0.0f, _verticalVelocity, 0.0f)*Time.deltaTime;
	}
}