using UnityEngine;
using UnityEngine.Animations;

public class CarryItem : MonoBehaviour, ICarriable
{
	[SerializeField] private Vector3 positionOffset;
	[SerializeField] private Vector3 rotationOffset;
	[SerializeField] [Range(0, 2f)] private float carriedScale = 1f;
	[SerializeField] private ScriptableEventForColliders onPlayerCarryStart;
	[SerializeField] private Transform leftHandTarget;
	[SerializeField] private Transform rightHandTarget;
	public bool cloneOnPickup;
	public bool destroyOnDrop;

	protected ParentConstraint Constraint;
	protected Vector3 OriginalScale { get; private set; }

	protected virtual bool CanPickUp() => !Constraint;

	/**
	 * Unity life-cycle
	 *
	 */
	public void Awake() {
		OriginalScale = transform.localScale;
	}

	protected void OnTriggerEnter(Collider other) {
		if (!other.CompareTag("Avatar") || !CanPickUp()) return;
		onPlayerCarryStart.TriggerEvent(new ColliderEventArgs {
			Other = other.gameObject,
			That = gameObject
		});
	}

	/**
	 * ICarriable
	 */
	public GameObject Pickup(Transform constraint) =>
		cloneOnPickup ? PickupClone(constraint) : PickupOriginal(constraint);

	public virtual void Drop() {
		if (destroyOnDrop) Destroy(gameObject);
		else {
			transform.localScale = OriginalScale;
			Destroy(Constraint);
		}
		//TODO: set on ground in the original position and rotation
	}

	public Transform LeftHandTarget() => leftHandTarget;

	public Transform RightHandTarget() => rightHandTarget;

	/**
	 * Internal features
	 *
	 */
	private GameObject PickupClone(Transform constraint) {
		CarryItem item = Instantiate(gameObject).GetComponent<CarryItem>();
		item.cloneOnPickup = false;
		item.destroyOnDrop = true;
		return item.Pickup(constraint);
	}

	private GameObject PickupOriginal(Transform constraint) {
		transform.localScale *= carriedScale;
		if (constraint != null) SetParentConstraint(constraint);
		return gameObject;
	}

	private void SetParentConstraint(Transform constraint) {
		Constraint = gameObject.AddComponent<ParentConstraint>();
		Constraint.rotationAtRest = Vector3.zero;
		Constraint.translationAtRest = Vector3.zero;
		Constraint.AddSource(new ConstraintSource {
			sourceTransform = constraint,
			weight = 1f
		});
		Constraint.rotationOffsets = new[] {
			rotationOffset
		};
		Constraint.translationOffsets = new[] {
			positionOffset
		};
		Constraint.constraintActive = true;
	}
}