using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public interface IAnimation
{
	public void Animate(Vector2 move, in Animator animator);

	public void Reset();
}

public interface IInverseKinematic
{
	public void Connect(in Animator animator);
	public void Disconnect();
}

public interface IPlayable
{
	public void Play(AnimationClip animationClip);

	public void Stop();
}

public partial class AvatarAnimation : AvatarBehaviour, IAvatarPlugin, IPlayable
{
	[SerializeField] private Animator animator;

	private Vector2 _move;
	private PlayableGraph _graph;
	private IAnimation _animation;
	private List<IInverseKinematic> _inverseKinematics = new List<IInverseKinematic>();

	/**
	 * IAvatarPlugin
	 *
	 */
	public void OnEnable() {
		avatar.OnMove += Move;
		avatar.OnTick += UpdateAnimation;
	}

	public void OnDisable() {
		avatar.OnMove -= Move;
		avatar.OnTick -= UpdateAnimation;
		if (_graph.IsValid()) _graph.Destroy();
	}

	/**
	 * Helpers
	 */
	public static void Override(in Animator animator, Animator overrideWith) {
		animator.runtimeAnimatorController = overrideWith.runtimeAnimatorController;
		overrideWith.enabled = false;
	}

	public void SetAnimator(Animator newAnimator) => animator = newAnimator;


	/**
	 * IAvatarPlugin
	 *
	 */
	public void Register() {
		_animation = GetComponentInChildren<IAnimation>();
		_inverseKinematics = GetComponentsInChildren<IInverseKinematic>().ToList();
		foreach (IInverseKinematic inverseKinematic in _inverseKinematics) inverseKinematic.Connect(in animator);
	}

	public void Unregister() {
		_animation?.Reset();
		if (animator != null) animator.runtimeAnimatorController = null;
		foreach (IInverseKinematic inverseKinematic in _inverseKinematics) inverseKinematic.Disconnect();
	}


	/**
	 * Event listeners
	 *
	 */
	private void Move(Vector2 move) => _move = move;


	/**
	 * IAnimation
	 *
	 */
	private void UpdateAnimation() {
		animator.enabled = active;
		_animation?.Animate(_move, in animator);
	}

	/**
	 * IPlayable
	 *
	 */
	public void Play(AnimationClip animationClip) =>
		AnimationPlayableUtilities.PlayClip(animator, animationClip, out _graph);

	public void Stop() {
		if (_graph.IsValid()) _graph.Destroy();
	}
}