using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

//TODO: clean up camera focus functionality
public class CameraFocus : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera targetCamera;
	[SerializeField] private float cameraBlendInTime;
	[SerializeField] private float cameraBlendOutTime;
	[SerializeField] private float timeSlowDownDuration;
	[SerializeField] private AnimationCurve timeScale;

	private Action _onTargetShown;

	public void Focus(GameObject target, Action onTargetShown) {
		if (Camera.main == null) return;
		// get the camera controlling component
		var brain = Camera.main.GetComponent<CinemachineBrain>();
		// cache initial values
		_onTargetShown = onTargetShown;
		bool ignoreTimescale = brain.m_IgnoreTimeScale;
		float defaultBlendTime = brain.m_DefaultBlend.m_Time;
		brain.m_IgnoreTimeScale = true;
		brain.m_DefaultBlend.m_Time = cameraBlendInTime;
		// set the target
		targetCamera.Follow = target.transform;
		targetCamera.LookAt = target.transform;
		targetCamera.Priority = 1000;
		targetCamera.enabled = true;
		// animate timescale
		DOVirtual.Float(0f, timeScale[timeScale.length - 1].time, timeSlowDownDuration,
			t => Time.timeScale = Mathf.Clamp01(timeScale.Evaluate(t))).SetUpdate(true).OnComplete(OnTimeScaleAnimated);
		return;

		void OnTimeScaleAnimated() {
			// restore timescale before invoking the callback
			Time.timeScale = 1f;
			_onTargetShown?.Invoke();
			// zoom back to default camera
			brain.m_DefaultBlend.m_Time = cameraBlendOutTime;
			targetCamera.enabled = false;
			brain.m_IgnoreTimeScale = ignoreTimescale;
			brain.m_DefaultBlend.m_Time = defaultBlendTime;
		}
	}
}