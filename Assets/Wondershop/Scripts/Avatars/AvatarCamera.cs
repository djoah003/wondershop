using UnityEngine;

public class AvatarCamera : MonoBehaviour, ICamera
{
	private Camera _camera;

	public bool TryGetCamera(out Camera avatarCamera) {
		// movement is often related to camera, as the player is deciding the direction based on the viewport.
		// currently, we default to the "MainCamera" that there is possibility that this is not always the case
		if (_camera == null) _camera = Camera.main;
		avatarCamera = _camera;
		return avatarCamera != null;
	}
}