using UnityEngine;
using UnityEngine.UI;

public class VignetteTransition : MonoBehaviour, ITransition
{
    [SerializeField] private Image vignette;
    [SerializeField] private float durationEnter;
    [SerializeField] private float durationExit;
    [SerializeField, Range(0f, 0.5f)] private float softness = 0.5f;

    private static readonly int EdgeOuter = Shader.PropertyToID("_EdgeOuter");
    private static readonly int EdgeInner = Shader.PropertyToID("_EdgeInner");

    private Material _material;
    private float _progress;
    private float _target;
    private float _timer;


    /**
     * Unity life-cycle
     * 
     */
    private void Awake() => _material = vignette.material;

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0) OnComplete();
        _progress = _target == 0f ? _timer / durationEnter : (durationExit - _timer) / durationExit;
        UpdateValues(_progress);
    }

    /**
     * Internal implementation
     * 
     */
    private void UpdateValues(float value)
    {
        _material.SetFloat(EdgeOuter, value);
        _material.SetFloat(EdgeInner, value - softness);
    }

    private void OnComplete() => enabled = false;


    /**
     * Transition interface
     * 
     */
    public float Open()
    {
        _timer = durationEnter;
        _target = 1.0f;
        enabled = true;
        return _timer;
    }

    public float Close()
    {
        _timer = durationExit;
        _target = 0.0f;
        enabled = true;
        return _timer;
    }

    public void SetOpen() => UpdateValues(1f);

    public void SetClosed() => UpdateValues(0f);
}