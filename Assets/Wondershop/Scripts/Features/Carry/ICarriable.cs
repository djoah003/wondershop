using UnityEngine;

public interface ICarriable
{
	public GameObject Pickup(Transform constraint);
	public void Drop();
	public Transform LeftHandTarget();
	public Transform RightHandTarget();
}