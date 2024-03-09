using UnityEngine;

public class EventConnect : MonoBehaviour
{
    private void Awake() => EventRegister.Connect(gameObject);
}