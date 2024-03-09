using UnityEngine;

public class Blink : MonoBehaviour
{
	[SerializeField] private GameObject[] elements;
	[SerializeField] [Range(0f, 1f)] private float blinkInterval;
	private bool _blink;

	private void OnEnable() => InvokeRepeating(nameof(Toggle), blinkInterval, blinkInterval);

	private void OnDisable() {
		CancelInvoke(nameof(Toggle));
		foreach (GameObject element in elements) element.SetActive(true);
	}

	private void Toggle() {
		foreach (GameObject element in elements) element.SetActive(!_blink);
		_blink = !_blink;
	}
}