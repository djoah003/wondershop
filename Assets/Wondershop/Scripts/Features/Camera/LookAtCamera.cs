using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	[SerializeField] private bool localRotation = false;
	private GameObject _target;

	private void Start() {
		Camera mainCamera = Camera.main;
		if (mainCamera) _target = mainCamera.gameObject;
	}

	private void Update() => FaceCamera();

	private void FaceCamera() {
		if (!_target) _target = GameObject.FindGameObjectWithTag("MainCamera");
		else transform.LookAt(transform.position + _target.transform.forward, localRotation ? transform.up : _target.transform.up);
	}
}