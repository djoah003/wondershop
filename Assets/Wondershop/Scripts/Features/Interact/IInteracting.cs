using UnityEngine;

public interface IInteracting
{
	public void EnterInteraction(GameObject target);
	public void StartInteraction(GameObject target);
	public void EndInteraction(GameObject target);
	public void ExitInteraction(GameObject target);
}