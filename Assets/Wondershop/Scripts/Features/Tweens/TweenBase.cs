using UnityEngine;
using DG.Tweening;
using System;

public class TweenBase : MonoBehaviour
{
    [Tooltip("Should the animation begin when spawned?")] [SerializeField]
    protected bool autoPlay = true;

    [Tooltip("What kind of ease should the animation use? Don't use the INTERNAL types!")] [SerializeField]
    protected Ease easeType = Ease.Linear;

    [Tooltip("Time in seconds to do one animation.")] [SerializeField]
    protected float animationTime = 1;

    [Tooltip("Infinite is -1, otherwise will loop this many times.")] [SerializeField]
    protected int loopCount = -1;

    [Tooltip("How should the animation loop?")] [SerializeField]
    protected LoopType loopType;

    [Tooltip("Should the object be disabled when animation is done?")] [SerializeField]
    protected bool disableOnDone;

    protected virtual void Awake()
    {
        if (autoPlay) Animate();
    }

    protected void OnDestroy() => transform.DOKill();

    public virtual void Animate()
    {
    }

    public virtual void Reset()
    {
        transform.DOKill(false);
        gameObject.SetActive(true);
    }

    public void StopAnimation() => transform.DOKill(true);

    protected void OnComplete()
    {
        if (disableOnDone) gameObject.SetActive(false);
    }
}