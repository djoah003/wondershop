using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FpsMonitor : MonoBehaviour
{
	private int _fpsDelta;
	private int _fpsFrame;
	private int _fpsFinal;
	private readonly GUIStyle _guiStyle = new GUIStyle();

	private float _fpsTimer;
	private const float FPSReportRate = 30;
	private readonly List<int> _fpsSamples = new List<int>();

	private void Start() {
		_guiStyle.fontSize = 10;
		_guiStyle.fontStyle = FontStyle.Bold;
		_guiStyle.normal.textColor = Color.white;
		_fpsTimer = FPSReportRate;
	}

	private void Update() {
		// track the frames and time
		_fpsFrame++;
		_fpsDelta += Mathf.RoundToInt(1f/Time.unscaledDeltaTime);
		_fpsTimer -= Time.unscaledDeltaTime;
		// if not enough samples, return early
		if (_fpsFrame%60 != 0) return;
		_fpsFinal = _fpsDelta/_fpsFrame;
		_fpsFrame = 0;
		_fpsDelta = 0;
		// collect reporting samples
		_fpsSamples.Add(_fpsFinal);
		if (_fpsTimer <= 0.0) Report();
	}

	private void OnGUI() =>
		GUI.Label(new Rect(10, 10, 100, 10), _fpsFinal + "FPS", _guiStyle);

	private void Report() {
		int min = _fpsSamples.Min();
		int max = _fpsSamples.Max();
		int avg = _fpsSamples.Sum()/_fpsSamples.Count;
		// report the values
		ProjectLogger.Log($"min: {min}, max: {max}, avg: {avg}");
		// reset reporting timer and clear samples
		_fpsTimer = FPSReportRate;
		_fpsSamples.Clear();
	}
}