using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDisplay
{
	public void Display(ref Bounds bounds, ref Camera cam, ref float offset, ref IModel model, bool display = true);
}

/// <summary>
/// AvatarDisplay handles the setup and updating of the canvas UI elements. It will use the character controller
/// as a reference point, so elements will always relate to the CharacterController bounds for the positioning.
/// The given display elements can either stack or be on top of each other by using the given offset or bounds.
/// </summary>
public class AvatarDisplay : AvatarBehaviour, IAvatarPlugin, IVisible
{
	private IModel _model;
	private CharacterController _controller;
	private ICamera _camera;
	private List<IDisplay> _displays = new List<IDisplay>();

	/**
	 * Unity life-cycle
	 *
	 */
	public void OnEnable() => avatar.OnLateTick += LateUpdateDisplay;

	public void OnDisable() => avatar.OnLateTick -= LateUpdateDisplay;

	/**
	 * Helpers
	 *
	 */
	public static void Billboard(ref Bounds bounds, ref Camera cam, ref Canvas canvas, float height) {
		// get the current maximum values for bounds
		Vector3 boundsMax = bounds.max;
		// get current camera rotation
		Quaternion rotation = cam.transform.rotation;
		// place the canvas relative to the Avatar, with the given offset (up by height)
		canvas.transform.position = new Vector3(bounds.center.x, boundsMax.y, boundsMax.z) + rotation*Vector3.up*height;
		// face the canvas towards the camera
		canvas.transform.LookAt(canvas.transform.position + rotation*Vector3.forward, rotation*Vector3.up);
	}

	public static void FixedSize(ref Camera cam, ref Canvas canvas, float originalScale, float minSize) {
		// get the current viewport size based on camera
		float viewportSize = cam.WorldToViewportPoint(canvas.transform.position + cam.transform.up).y -
			cam.WorldToViewportPoint(canvas.transform.position).y;
		// get the scale of the object
		float size = viewportSize < minSize ? minSize/viewportSize : 1;
		// possibility to clamp scale? Mathf.Clamp(relativeScale / viewportSize, minScale, maxScale);
		// finally, apply the given scale
		canvas.transform.localScale = Vector3.one*(originalScale*size);
	}

	/**
	 * IAvatarPlugin
	 *
	 */
	public void Register() {
		_model = avatar.Model;
		_camera = avatar.mainCamera;
		_displays = GetComponentsInChildren<IDisplay>().ToList();
		_controller = GetComponent<CharacterController>();
	}

	public void Unregister() => _displays.Clear();


	/***
	 * IVisible
	 *
	 */
	public void SetVisibility(bool value) => active = value;

	/**
	 * IDisplay
	 *
	 */
	private void LateUpdateDisplay() {
		if (_camera == null || !_camera.TryGetCamera(out Camera avatarCamera)) return;
		float offset = 0f;
		Bounds bounds = _controller.bounds;
		foreach (IDisplay display in _displays)
			display?.Display(ref bounds, ref avatarCamera, ref offset, ref _model, active);
	}
}