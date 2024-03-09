using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EventCollider : MonoBehaviour
{
    [SerializeField] private ScriptableEventForColliders onCollider;

    private void OnControllerColliderHit(ControllerColliderHit other) =>
        onCollider.TriggerEvent(new ColliderEventArgs() { That = gameObject, Other = other.gameObject });
}