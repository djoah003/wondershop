using TMPro;
using UnityEngine;

/// <summary>
/// AvatarDisplayNameLabel will handle both custom player name and the in-game display of the player index.
/// The custom player name can be toggled on and off.
/// </summary>
public class AvatarDisplayNameLabel : AvatarBehaviour, IDisplay
{
	[SerializeField] private Canvas label;
	[SerializeField] private float scale = 0.05f;
	[SerializeField] private float minViewportSize = 0.1f;
	[SerializeField] private TMP_Text playerLabel;

	private float? _height;

	/**
	 * IDisplay
	 *
	 */
	public void Display(ref Bounds bounds, ref Camera cam, ref float offset, ref IModel model, bool display) {
		label.enabled = active && display;
		if (!active || model == null) return;
		// update the canvas text
		playerLabel.text = model.Player().DisplayName.Equals(string.Empty) ? $"Player {model.Index()}" : model.Player().DisplayName;
		playerLabel.gameObject.SetActive(playerLabel.text.Length != 0);
		// run additional transforms
		_height ??= label.transform.position.y - bounds.max.y;
		AvatarDisplay.Billboard(ref bounds, ref cam, ref label, _height.Value);
		AvatarDisplay.FixedSize(ref cam, ref label, scale, minViewportSize);
	}
}