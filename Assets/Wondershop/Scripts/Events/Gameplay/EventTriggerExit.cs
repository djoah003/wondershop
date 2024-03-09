using UnityEngine;

public class EventTriggerExit : MonoBehaviour
{
    [SerializeField] private ScriptableEventForColliders onTriggerExit;

    private void OnTriggerExit(Collider other) => onTriggerExit.TriggerEvent(new ColliderEventArgs()
        { That = gameObject, Other = other.gameObject });
}