using UnityEngine;


public class AvatarAnimationDefault : AvatarBehaviour, IAnimation
{
	[SerializeField] private Animator overrideWith;
	private static readonly int MoveParameter = Animator.StringToHash("Move");
	private static readonly int IdleParameter = Animator.StringToHash("Idle");
	private float _idleTime;

	/**
	 * IAnimation
	 *
	 */
	public void Animate(Vector2 move, in Animator animator) {
		if (overrideWith.Equals(null) || !animator.gameObject.activeSelf) return;
		// if local animator is still enabled, override global animator
		if (overrideWith.enabled) AvatarAnimation.Override(in animator, overrideWith);
		// resolve if we have should be moving
		bool isMoving = move != Vector2.zero;
		// toggle the running animation
		animator.SetBool(MoveParameter, isMoving);
		// set the idle time counter
		if (isMoving) _idleTime = 0f;
		else _idleTime += Time.deltaTime;
		animator.SetInteger(IdleParameter, (int)_idleTime);
	}

	public void Reset() {
		if (overrideWith.Equals(null)) return;
		overrideWith.enabled = true;
	}
}