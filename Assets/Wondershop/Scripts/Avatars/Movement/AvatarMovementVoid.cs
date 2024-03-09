using UnityEngine;

public class AvatarMovementVoid : AvatarBehaviour, IMovement
{
	[SerializeField] private float voidBoundaries = -100f;
	[SerializeField] private LayerMask groundLayers;
	private Camera _mainCamera;


	/**
	 * IMovement
	 *
	 */
	public void Move(Vector2 move, bool press, bool hold, ref Vector3 moveNext, ref CharacterController controller,
		ref Camera cam) {
		if (!active) return;
		// checks if player has been falling to super far, if yes, return to somewhere withing the camera
		if (voidBoundaries < controller.transform.position.y) return;
		// get position based on camera view, from the center of the camera
		Ray ray = cam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		// raycast to possible spots, take into account correct layers
		Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayers);
		// transform player to hit or by default to zero
		moveNext = Vector3.zero;
		controller.transform.position = hit.point;
	}
}