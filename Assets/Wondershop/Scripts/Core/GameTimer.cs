using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
	public UnityEvent<float> onTimer;

	[UsedImplicitly]
	public void OnTimer(float value) => onTimer?.Invoke(value);
}