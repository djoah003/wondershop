using UnityEngine;

public class AvatarMovementYJump : AvatarBehaviour, IMovement, IAnimation
{
	[Tooltip("The character uses its own gravity value.")]
	public float gravity = -9.81f;

	[Tooltip("When let go of the button, up velocity is slowed down with this multiplier.")]
	public float lowJumpMultiplier = 1.75f;

	[Tooltip("Gravity multiplier when falling.")]
	public float fallMultiplier = 1.5f;

	[Tooltip("The height the player can jump")]
	public float jumpHeight = 2f;

	[Tooltip("What layers the character uses as ground")]
	public LayerMask groundLayers;

	[SerializeField] private Animator overrideWith;

	private static readonly int MoveParameter = Animator.StringToHash("Move");
	private static readonly int JumpParameter = Animator.StringToHash("Jump");

	private float _idleTime;
	private bool _jumping;
	private float _verticalVelocity;

	/**
	 * IMovement
	 *
	 */
	public void Move(Vector2 move, bool press, bool hold, ref Vector3 moveNext, ref CharacterController controller,
		ref Camera cam) {
		if (!active) return;
		bool grounded = AvatarMovement.Grounded(controller, groundLayers, offset: 0f);
		// jump on ground and button press
		if (press && grounded) Jump();
		// apply gravity
		_verticalVelocity += gravity*Time.deltaTime;
		if (grounded && _verticalVelocity < 0) {
			if (_jumping) Land();
			// zero out velocity when grounded
			_verticalVelocity = 0;
		} else if (_verticalVelocity < 0) {
			// add fall multiplied velocity when falling, used to speed up falling speed
			_verticalVelocity += gravity*(fallMultiplier - 1)*Time.deltaTime;
		} else if (_verticalVelocity > 0 && !hold) {
			//A ad lowJump multiplied velocity when falling and not holding jump button, used to make a smaller jump
			_verticalVelocity += gravity*(lowJumpMultiplier - 1)*Time.deltaTime;
		}
		moveNext += new Vector3(0, _verticalVelocity, 0)*Time.deltaTime;
	}

	private void Jump() {
		_jumping = true;
		_verticalVelocity += Mathf.Sqrt(jumpHeight*-2f*gravity);
	}

	private void Land() => _jumping = false;


	/**
	 * IAnimation
	 *
	 */
	public void Animate(Vector2 move, in Animator animator) {
		if (overrideWith.Equals(null)) return;
		// if local animator is still enabled, override global animator
		if (overrideWith.enabled) AvatarAnimation.Override(in animator, overrideWith);
		// resolve if we have should be moving
		bool isMoving = move != Vector2.zero;
		// toggle the running animation
		animator.SetBool(MoveParameter, isMoving);
		animator.SetBool(JumpParameter, _jumping);
	}

	public void Reset() {
		if (overrideWith.Equals(null)) return;
		overrideWith.enabled = true;
	}
}