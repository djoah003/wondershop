using UnityEngine;
using DG.Tweening;

public class TweenRotation : TweenBase
{
    [Tooltip("How much should it rotate?")] [SerializeField]
    private Vector3 rotationAmounts;
    
    private Quaternion _startRotation;

    protected override void Awake()
    {
        base.Awake();
        _startRotation = transform.localRotation;
    }
    
    public override void Animate()
    {
        transform.DOLocalRotate(rotationAmounts, animationTime, RotateMode.FastBeyond360)
            .SetLoops(loopCount, loopType)
            .SetRelative()
            .SetEase(easeType)
            .onComplete += OnComplete;
    }

    public override void Reset()
    {
        base.Reset();
        transform.DOLocalRotate(_startRotation.eulerAngles, animationTime).SetEase(easeType);
    }
}