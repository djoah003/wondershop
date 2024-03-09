using UnityEngine;

public class AvatarMovementXZ : AvatarBehaviour, IMovement, ISpeed
{
	[Tooltip("Move speed of the character in m/s")] [SerializeField]
	private float movementSpeed = 8.0f;

	private float _speed;
	private float _velocity;
	private float _defaultSpeed;
	private float _speedMultiplier = 1.0f;

	/**
	 * IMovement
	 *
	 */
	public void Move(Vector2 input, bool press, bool hold, ref Vector3 moveNext, ref CharacterController controller,
		ref Camera cam) {
		if (!active) return;
		_speed = movementSpeed;
		// if there is no input, set the target speed to 0
		if (input == Vector2.zero) _speed = 0.0f;
		// the given input direction
		Vector3 inputDirection = new Vector3(input.x, 0.0f, input.y).normalized;
		// direction relative to camera (up should always be up)
		float relativeDirection =
			Mathf.Atan2(inputDirection.x, inputDirection.z)*Mathf.Rad2Deg + cam.transform.eulerAngles.y;
		// calculate the new vector based on direction
		Vector3 targetDirection = Quaternion.Euler(0.0f, relativeDirection, 0.0f)*Vector3.forward;
		// apply the movement for this frame (nextMove)
		moveNext += targetDirection*(_speed*Time.deltaTime);
	}


	/**
	 * ISpeed
	 *
	 */
	public float GetSpeed() => movementSpeed;

	public void SetSpeed(float speed) {
		if (_defaultSpeed == 0f) _defaultSpeed = movementSpeed;
		movementSpeed = speed*_speedMultiplier;
	}

	public void SetSpeedMultiplier(float multiplier = 1.0f, bool force = false) {
		if (!force && multiplier < _speedMultiplier) return;
		_speedMultiplier = multiplier;
		movementSpeed = _speed*_speedMultiplier;
	}
}