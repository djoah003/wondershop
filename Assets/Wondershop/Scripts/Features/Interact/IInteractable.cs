using UnityEngine;

public interface IInteractable
{
	public void StartInteraction(GameObject source);
	public void EndInteraction(GameObject source);
	public void DoneInteraction(GameObject source);
	public AnimationClip InteractionAnimation();
}