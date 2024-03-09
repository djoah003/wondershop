using UnityEngine;

public class AvatarCharacterVariant : MonoBehaviour
{
	[SerializeField] private Renderer target;
	[SerializeField] private Material[] materials;

	/**
	 * Feature implementation
	 *
	 */
	private void SetCharacterMaterial(int index) {
		// check that material actually exists
		if (materials.Length == 0) return;
		// if index is more than materials, clamp it
		if (materials.Length < index) index -= (materials.Length/index)*index;
		// get the material and set it to target
		target.material = materials[index - 1];
	}

	/**
	 * Public interface
	 *
	 */
	public void SetVariant(int variant) => SetCharacterMaterial(variant);
}