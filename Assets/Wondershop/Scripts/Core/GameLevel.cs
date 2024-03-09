using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class GameLevel : MonoBehaviour
{
	private ISequence _sequence;
	[SerializeField] private GameObject levelStartSequence;

	private static bool ShouldPlay() {
#if UNITY_EDITOR
		return EditorPrefs.GetBool("WondershopSettings.PlayStartSequence");
#else
        return true;
#endif
	}

	public void OnStateEnter(Action onComplete) {
		levelStartSequence.TryGetComponent(out _sequence);
		OnLevelStart(onComplete);
	}

	protected virtual void OnLevelStart(Action onComplete) => onComplete?.Invoke();
	protected virtual void OnLevelEnd(Action onComplete) => onComplete?.Invoke();

	public void OnStateExit(Action onComplete) => OnLevelEnd(onComplete);

	protected void PlayStartSequence(Action onComplete) {
		if (ShouldPlay() && _sequence != null) _sequence.Begin(onComplete);
		else onComplete?.Invoke();
	}
}