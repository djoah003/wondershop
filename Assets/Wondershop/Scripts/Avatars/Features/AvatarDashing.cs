using UnityEngine;


public class AvatarDashing : AvatarBehaviour, IMovement, IRotation
{
	[SerializeField] private float speedup;
	[SerializeField] private AnimationCurve speedupCurve;
	[SerializeField] private float timeout;
	[SerializeField] private float cooldown;
	[SerializeField] private AnimationCurve cooldownCurve;
	[SerializeField] private TrailRenderer trail;

	private float _dashTimeouts;
	private float _dashCooldown;
	private Quaternion _dashRotation = Quaternion.identity;


	/**
	 * Helpers
	 *
	 */
	private bool IsDashing => 0f < _dashTimeouts;

	private Vector3 OnCooldown(Vector3 move) {
		float cooldownProgress = 1f - Mathf.Clamp01(_dashCooldown/(timeout + cooldown));
		_dashCooldown -= Time.deltaTime;
		return move*cooldownCurve.Evaluate(cooldownProgress);
	}

	private Vector3 OnDashUpdate(ref CharacterController controller) {
		float dashProgress = 1f - Mathf.Clamp01(_dashTimeouts/timeout);
		_dashTimeouts -= Time.deltaTime;
		return controller.transform.forward*(speedupCurve.Evaluate(dashProgress)*speedup*Time.deltaTime);
	}

	private void OnDashDirection(Vector2 move, Component cam) => _dashRotation =
		Quaternion.Euler(0f, Mathf.Atan2(move.x, move.y)*Mathf.Rad2Deg + cam.transform.eulerAngles.y, 0f);

	private Vector3 OnDashEnable(Vector2 move, bool press, bool hold, ref Vector3 moveNext, ref CharacterController controller, ref Camera cam) {
		// dash is activated with press
		if (!press) return moveNext;
		// if pressed, start the dash
		_dashTimeouts = timeout;
		_dashCooldown = timeout + cooldown;
		_dashRotation = controller.transform.rotation;
		// take direction from input if we're getting a proper value
		if (0.1f < move.sqrMagnitude) OnDashDirection(move, cam);
		// finally return the actual dash movement
		return OnDashUpdate(ref controller);
	}


	/**
	 * IMovement
	 *
	 */
	public void Move(Vector2 move, bool press, bool hold, ref Vector3 moveNext, ref CharacterController controller, ref Camera cam) {
		if (!active) return;
		// if dash timeout is still running, keep dashing player with given configuration
		if (trail != null) trail.emitting = IsDashing;
		if (IsDashing) moveNext = OnDashUpdate(ref controller);
		// if cooldown is still running, return the base movement else check for press
		else moveNext = IsDashing ? OnCooldown(moveNext) : OnDashEnable(move, press, hold, ref moveNext, ref controller, ref cam);
	}

	/**
	 * IRotation
	 *
	 */
	public void Rotate(Vector2 move, ref Quaternion rotationNext, ref Camera cam) {
		if (!active) return;
		if (IsDashing) rotationNext = _dashRotation;
	}
}