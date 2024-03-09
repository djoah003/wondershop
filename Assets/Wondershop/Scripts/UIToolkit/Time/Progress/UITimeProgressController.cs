using UnityEngine;
using UnityEngine.UIElements;

public class UITimeProgressController
{
    private UnityEngine.UIElements.ProgressBar _progressBar;
    private Label _label;
    private VisualElement _container;
    private float _maxValue;

    /**
     * Text formatting
     *
     */
    private static string Format(float time)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(time));
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    /**
     * GUI management
     *
     */
    public void SetVisualElement(VisualElement visualElement)
    {
        _container = visualElement;
        _progressBar = _container.Q<UnityEngine.UIElements.ProgressBar>("progress");
        _label = _container.Q<Label>("TimeLabel");
    }

    public void SetTime(float time)
    {
        if (_maxValue.Equals(null) || _maxValue < time) _maxValue = time;
        _label.text = Format(time);
        _progressBar.value = time / _maxValue * 100;
    }
}