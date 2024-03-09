using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AvatarInteracting : AvatarBehaviour, IAction, IInteracting
{
	[SerializeField] private ScriptableEventForColliders onPlayerInteractionStart;
	[SerializeField] private ScriptableEventForColliders onPlayerInteractionEnd;

	private PlayableGraph _graph;
	private IInteracting _interactingImplementation;
	private readonly List<GameObject> _targets = new List<GameObject>();
	private readonly List<GameObject> _interacting = new List<GameObject>();

	public void ActionInput(Vector2 move, bool press, bool hold) {
		// TODO: do a minor cleanup
		// check if any targets exists
		if (_targets.Count == 0) return;
		var indexesToRemove = new List<int>();
		for (int i = 0; i < _targets.Count; ++i) {
			GameObject target = _targets[i];
			if (!target) {
				indexesToRemove.Add(i);
				continue;
			}
			HoldAction(hold, target);
		}

		//remove disappeared targets
		foreach (int index in indexesToRemove) {
			_targets.RemoveAt(index);
		}
	}

	private void HoldAction(bool hold, GameObject target) {
		if (hold && !_interacting.Contains(target)) StartInteraction(target);
		if (_interacting.Contains(target) && !hold) EndInteraction(target);
	}

	public void EnterInteraction(GameObject target) => _targets.Add(target);

	public void StartInteraction(GameObject target) {
		_interacting.Add(target);
		onPlayerInteractionStart.TriggerEvent(new ColliderEventArgs {
			Other = target,
			That = gameObject,
		});
	}

	public void EndInteraction(GameObject target) {
		_interacting.Remove(target);
		if (_interacting.Count == 0)
			onPlayerInteractionEnd.TriggerEvent(new ColliderEventArgs {
				Other = target,
				That = gameObject,
			});
	}

	public void ExitInteraction(GameObject target) {
		if (_interacting.Contains(target)) EndInteraction(target);
		_targets.Remove(target);
	}
}