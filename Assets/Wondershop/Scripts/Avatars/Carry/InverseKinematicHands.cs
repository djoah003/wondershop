using UnityEngine;

public class InverseKinematicHands : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private Transform handL;
	[SerializeField] private Transform handR;

	/**
	 *  Unity life-cycle
	 */
	public void OnAnimatorIK(int layerIndex) {
		animator = GetComponent<Animator>();
		SetHand(AvatarIKGoal.LeftHand, handL);
		SetHand(AvatarIKGoal.RightHand, handR);
	}

	private void SetHand(AvatarIKGoal goal, Transform hand) {
		animator.SetIKPositionWeight(goal, hand == null ? 0 : 1);
		animator.SetIKRotationWeight(goal, hand == null ? 0 : 1);
		if (hand == null) return;
		animator.SetIKPosition(goal, hand.position);
		animator.SetIKRotation(goal, hand.rotation);
	}

	/**
	 * External API
	 *
	 */
	public void SetHandL(Transform leftHand) => handL = leftHand;

	public void SetHandR(Transform rightHand) => handR = rightHand;
}