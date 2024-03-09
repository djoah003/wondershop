using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableArea : MonoBehaviour, IInteractable
{
	[SerializeField] private AnimationClip interactionAnimation;
	[SerializeField] private ScriptableEventForColliders onPlayerInteractionEnter;
	[SerializeField] private ScriptableEventForColliders onPlayerInteractionExit;
	[SerializeField] protected ScriptableEventForColliders onPlayerInteractionEnd;
	[SerializeField] protected ScriptableEventForColliders onPlayerInteractionDone;
	protected readonly List<GameObject> interacting = new List<GameObject>();
	/**
	 * Unity life-cycle
	 *
	 */
	protected virtual void OnTriggerEnter(Collider other) {
		if (!other.CompareTag("Avatar")) return;
		onPlayerInteractionEnter.TriggerEvent(new ColliderEventArgs { Other = other.gameObject, That = gameObject });
	}

	protected virtual void OnTriggerExit(Collider other) {
		if (!other.CompareTag("Avatar")) return;
		onPlayerInteractionExit.TriggerEvent(new ColliderEventArgs { Other = other.gameObject, That = gameObject });
	}

	/**
	 * IInteractable
	 *
	 */
	public virtual void StartInteraction(GameObject source) => interacting.Add(source);
	public virtual void EndInteraction(GameObject source) => interacting.Remove(source);
	public virtual void DoneInteraction(GameObject source){}
	public virtual AnimationClip InteractionAnimation() => interactionAnimation;
	
	protected void CompleteInteraction(GameObject player) {
		onPlayerInteractionEnd.TriggerEvent(new ColliderEventArgs { Other = gameObject, That = player });
		onPlayerInteractionDone.TriggerEvent(new ColliderEventArgs { Other = gameObject, That = player });
	}
}