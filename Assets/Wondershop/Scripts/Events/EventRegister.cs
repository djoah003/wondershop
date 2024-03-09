using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public static class EventRegister
{
	public static void Connect(GameObject gameObject) {
		Component[] components = gameObject.GetComponents(typeof(Component));
		foreach (Component component in components) {
			MethodInfo[] methods = component.GetType()
				.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public |
					BindingFlags.Instance);
			foreach (MethodInfo method in methods) {
				if (Attribute.GetCustomAttribute(method, typeof(EventListener)) is EventListener)
					LoadAsset(gameObject, component, method);
			}
		}
	}

	private static void LoadAsset(GameObject gameObject, Component component, MethodInfo method) {
		if (method.IsPrivate) Debug.LogError($"Could not connect {method.Name}. Method is private.");
		Object scriptableEvent = Resources.Load($"Events/{method.Name}");
		if (scriptableEvent == null) Debug.LogError($"Could not connect {method.Name}. ScriptableEvent not found.");
		else CreateObserver(gameObject, component, method, scriptableEvent);
	}

	// TODO: currently creates duplicate observers, update to combine them
	private static void CreateObserver(GameObject gameObject, Component component, MethodInfo method,
		Object scriptableEvent) {
		if (scriptableEvent is ScriptableEventForColliders forColliders)
			EventObserverForColliders(gameObject, component, method, forColliders);
		if (scriptableEvent is ScriptableEventForGameObject forGameObject)
			EventObserverForGameObject(gameObject, component, method, forGameObject);
	}

	/**
	 * Collider Event handling
	 */
	private static UnityAction<ColliderEventArgs> ColliderEventDelegate(Component component, MethodInfo method) =>
		(UnityAction<ColliderEventArgs>)Delegate.CreateDelegate(typeof(UnityAction<ColliderEventArgs>), component,
			method);

	private static void EventObserverForColliders(GameObject gameObject, Component component, MethodInfo method,
		ScriptableEventForColliders scriptableEvent) {
		EventsObserverForColliders eventObserver = gameObject.AddComponent<EventsObserverForColliders>();
		eventObserver.enabled = false;
		eventObserver.scriptableEvent = scriptableEvent;
		eventObserver.onEvent = new UnityEvent<ColliderEventArgs>();
#if UNITY_EDITOR
		UnityEventTools.AddPersistentListener(eventObserver.onEvent, ColliderEventDelegate(component, method));
#else
        eventObserver.onEvent.AddListener(ColliderEventDelegate(component, method));
#endif
		eventObserver.enabled = true;
	}

	/**
	 * GameObject Event handling
	 */
	private static UnityAction<GameObject> GameObjectEventDelegate(Component component, MethodInfo method) =>
		(UnityAction<GameObject>)Delegate.CreateDelegate(typeof(UnityAction<GameObject>), component,
			method);

	private static void EventObserverForGameObject(GameObject gameObject, Component component, MethodInfo method,
		ScriptableEventForGameObject scriptableEvent) {
		EventsObserverForGameObject eventObserver = gameObject.AddComponent<EventsObserverForGameObject>();
		eventObserver.enabled = false;
		eventObserver.scriptableEvent = scriptableEvent;
		eventObserver.onEvent = new UnityEvent<GameObject>();
#if UNITY_EDITOR
		UnityEventTools.AddPersistentListener(eventObserver.onEvent, GameObjectEventDelegate(component, method));
#else
        eventObserver.onEvent.AddListener(GameObjectEventDelegate(component, method));
#endif
		eventObserver.enabled = true;
	}
}