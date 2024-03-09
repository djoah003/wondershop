using UnityEngine;

public interface ICarrying
{
	public void Pickup(GameObject item);
	public void Drop(GameObject carryItem);
	public GameObject Item();
}