using UnityEngine;

public class AvatarRotationXZ : AvatarBehaviour, IRotation
{
	[Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)]
	public float rotationSmoothTime = 0.075f;
	private float _targetRotation;
	private float _rotationVelocity;


	/**
	 * IRotation
	 *
	 */
	public void Rotate(Vector2 move, ref Quaternion rotationNext, ref Camera cam) {
		if (move == Vector2.zero || !active) return;
		// get the XZ input direction from move input
		Vector3 direction = new Vector3(move.x, 0.0f, move.y).normalized;
		// resolve the rotation towards which player is turning
		_targetRotation = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg + cam.transform.eulerAngles.y;
		// timeScale 0 was causing rare assertions here when rotation came back NaN
		float rotation = Time.timeScale != 0
			? Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime)
			: transform.eulerAngles.y;
		// finally apply the rotation to the transform 
		rotationNext = Quaternion.Euler(0.0f, rotation, 0.0f);
	}
}