using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ScriptableEvent<T> : ScriptableObject
{
    private List<IEventObserver<T>> _observers = new();
    public void AddObserver(IEventObserver<T> observer) => _observers.Add(observer);
    public void RemoveObserver(IEventObserver<T> observer) => _observers.Remove(observer);

    public void TriggerEvent(T parameter)
    {
        foreach (IEventObserver<T> observer in _observers) observer.OnEventTriggered(parameter);
    }
}