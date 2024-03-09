using UnityEngine;

public class ProjectSingleton<T> : MonoBehaviour where T : Component
{
	private static T _instance;
	private static bool _quitting;

	public static T Instance {
		get {
			if (_instance != null) return _instance;
			_instance = (T)FindObjectOfType(typeof(T));
			if (_instance == null && !_quitting) SetupInstance();
			return _instance;
		}
	}

	private static void SetupInstance() {
		_instance = (T)FindObjectOfType(typeof(T));
		if (_instance != null) return;
		GameObject gameObj = new() {
			name = typeof(T).Name
		};
		_instance = gameObj.AddComponent<T>();
		DontDestroyOnLoad(gameObj);
	}

	private void RemoveDuplicates() {
		if (_instance == null) _instance = this as T;
		else Destroy(gameObject);
	}

	/**
	 * Unity life-cycle
	 *
	 */
	protected virtual void Awake() => RemoveDuplicates();

	protected virtual void OnApplicationQuit() => _quitting = true;
}