using UnityEngine;

public class EventTriggerEnter : MonoBehaviour
{
    [SerializeField] private ScriptableEventForColliders onTriggerEnter;

    private void OnTriggerEnter(Collider other) => onTriggerEnter.TriggerEvent(new ColliderEventArgs()
        { That = gameObject, Other = other.gameObject });

    public void SetOnTriggerEnter(ScriptableEventForColliders colliderEvent) => onTriggerEnter = colliderEvent;
}