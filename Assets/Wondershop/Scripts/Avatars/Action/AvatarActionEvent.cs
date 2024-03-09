using UnityEngine;
public class AvatarActionEvent : AvatarBehaviour, IAction
{
	[SerializeField] private ScriptableEventForGameObject OnPlayerPressEvent;
	[SerializeField] private ScriptableEventForGameObject OnPlayerHoldEvent;

	public void ActionInput(Vector2 move, bool press, bool hold) {
		if (press) OnPlayerPressEvent.TriggerEvent(gameObject);
		if (hold) OnPlayerHoldEvent.TriggerEvent(gameObject);
	}
}