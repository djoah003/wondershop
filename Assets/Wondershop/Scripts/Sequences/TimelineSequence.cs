using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineSequence : MonoBehaviour, ISequence
{
	[SerializeField] private float reduceDuration;

	public void Begin(Action onComplete) {
		PlayableDirector director = GetComponent<PlayableDirector>();
		director.Play();
		DOVirtual.DelayedCall((float)director.duration - reduceDuration, () => onComplete?.Invoke());
	}
}