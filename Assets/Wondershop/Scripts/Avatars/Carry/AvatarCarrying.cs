using UnityEngine;

public class AvatarCarrying : AvatarBehaviour, ICarrying, IInverseKinematic
{
	[SerializeField] private bool replaceOnPickup;
	[SerializeField] private Transform constraint;

	private GameObject _carrying;
	private Animator _animator;
	private InverseKinematicHands _inverseKinematic;

	/**
	 * Helpers
	 *
	 */
	/**
	 * Unity life-cycle
	 *
	 */
	private void OnEnable() {
		// reset constraint when the mesh changes?
		if (constraint != null) return;
		foreach (Transform child in transform.parent) {
			constraint = child.Find("Root/Hips");
			if (constraint != null) break;
		}
	}

	/**
	 * ICarrying
	 *
	 */
	public void Pickup(GameObject item) {
		// if replacing on pickup is allowed, drop previous item
		if (_carrying != null && replaceOnPickup) Drop(_carrying);
		// always check that carrying is null, otherwise return early
		if (_carrying != null) return;
		// check if we should copy the item, or use the original
		ICarriable carriedItem = item.GetComponent<ICarriable>();
		_carrying = carriedItem.Pickup(constraint);
		_inverseKinematic.SetHandL(_carrying.GetComponent<ICarriable>().LeftHandTarget());
		_inverseKinematic.SetHandR(_carrying.GetComponent<ICarriable>().RightHandTarget());
	}

	public void Drop(GameObject carryItem) {
		_carrying.GetComponent<ICarriable>().Drop();
		_carrying = null;
		_inverseKinematic.SetHandL(null);
		_inverseKinematic.SetHandR(null);
	}

	public GameObject Item() => _carrying;

	/**
	 * IInverseKinematics
	 *
	 */
	public void Connect(in Animator animator) {
		if (_inverseKinematic == null) _inverseKinematic = animator.gameObject.AddComponent<InverseKinematicHands>();
	}

	public void Disconnect() => Destroy(_inverseKinematic);
}