using UnityEngine;
using UnityEngine.Events;

public interface IEventObserver<in T>
{
    void OnEventTriggered(T parameter);
}

public abstract class EventsObserver<T, TScriptableEvent, TUnityEvent> : MonoBehaviour, IEventObserver<T>
    where TScriptableEvent : ScriptableEvent<T>
    where TUnityEvent : UnityEvent<T>
{
    public TScriptableEvent scriptableEvent;
    public TUnityEvent onEvent;

    private void OnEnable()
    {
        if (scriptableEvent == null) return;
        scriptableEvent.AddObserver(this);
    }

    private void OnDisable()
    {
        if (scriptableEvent == null) return;
        scriptableEvent.RemoveObserver(this);
    }

    public void OnEventTriggered(T parameter) => onEvent.Invoke(parameter);
}