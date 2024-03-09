using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class TweenPosition : TweenBase
{
    [Tooltip("How much should it move?")] [SerializeField]
    private Vector3 movementAmount;
    
    private Vector3 _startPosition;
    private Vector3 _startPositionLocal;

    protected override void Awake()
    {
        base.Awake();
        _startPosition = transform.position;
        _startPositionLocal = transform.localPosition;
    }

    public override void Animate()
    {
        transform.DOLocalMove(movementAmount, animationTime)
            .SetLoops(loopCount, loopType)
            .SetRelative()
            .SetEase(easeType)
            .onComplete += OnComplete;
    }

    public override void Reset()
    {
        base.Reset();
        transform.DOLocalMove(_startPositionLocal, animationTime).SetEase(easeType);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Handles.color = Color.blue;
        Vector3 start = Application.isPlaying ? _startPosition : transform.position;
        Vector3 end = Application.isPlaying ? _startPosition + movementAmount : transform.position + movementAmount;
        Handles.DrawLine(start, end, 3);
        Handles.CubeHandleCap(0, start, Quaternion.identity, 0.3f, EventType.Repaint);
        Handles.CubeHandleCap(1, end, Quaternion.identity, 0.3f, EventType.Repaint);
    }
#endif
}