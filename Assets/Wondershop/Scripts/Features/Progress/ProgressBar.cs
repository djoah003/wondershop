using UnityEngine;
using UnityEngine.UI;


public class ProgressBar : MonoBehaviour, IProgress
{
	[SerializeField] private GameObject progress;
	[SerializeField] private Image progressBarBg;
	[SerializeField] private Image progressBarFg;
	[SerializeField] private Gradient color;

	/**
	 * Unity life-cycleq
	 *
	 */
	private void Start() {
		progress.gameObject.SetActive(false);
		progressBarFg.fillAmount = 0f;
	}

	/**
	 * Feature API
	 *
	 */
	public void SetActive(bool value) {
		progress.SetActive(value);
	}

	public void Fill(float current = 0f, float max = 1f, bool inverse = false) {
		if (inverse) current = max - current;
		progress.gameObject.SetActive(0f < current);
		progressBarFg.fillAmount = current/max;
		progressBarFg.color = color.Evaluate(inverse ? 1f - progressBarFg.fillAmount : progressBarFg.fillAmount);
	}
}