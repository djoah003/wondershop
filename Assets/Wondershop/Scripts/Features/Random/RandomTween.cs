using UnityEngine;
using Random = UnityEngine.Random;

public class RandomTween : MonoBehaviour
{
    [Tooltip("Should tween trigger when Player exits trigger collider?")] [SerializeField]
    private bool triggerOnExit;

    [Tooltip("Should tween trigger randomly?")] [SerializeField]
    private bool random;

    [Tooltip("How long should we wait in start position?")] [SerializeField]
    private float timeStart;

    [Tooltip("How long should we wait in end position?")] [SerializeField]
    private float timeEnd;

    [Tooltip("When random is enabled, this is used to create randomness to the time.")] [SerializeField]
    private float randomStart;

    [Tooltip("When random is enabled, this is used to create randomness to the time.")] [SerializeField]
    private float randomEnd;

    [Tooltip("Reference to the Tween script")] [SerializeField]
    private TweenBase tween;

    private void Start()
    {
        Invoke(nameof(TweenStart), CalculateTime(timeEnd, randomEnd));
    }

    private void TweenStart()
    {
        tween.Animate();
        Invoke(nameof(TweenEnd), CalculateTime(timeStart, randomStart));
    }

    private void TweenEnd()
    {
        tween.Reset();
        Invoke(nameof(TweenStart), CalculateTime(timeEnd, randomEnd));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!triggerOnExit) return;
        if (!other.CompareTag("Player")) return;
        CancelInvoke(nameof(TweenEnd));
        TweenEnd();
    }

    private float CalculateTime(float baseTime, float addition)
    {
        float randomTime = random ? Random.Range(0.0f, addition) : 0.0f;
        return baseTime + randomTime;
    }
}