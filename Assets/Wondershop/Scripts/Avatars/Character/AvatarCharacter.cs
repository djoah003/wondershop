using UnityEngine;

public class AvatarCharacter : AvatarBehaviour, IAvatarPlugin, IVisible
{
	[SerializeField] private GameObject defaultCharacter;
	private bool _hide = true;
	private IModel _model;
	private GameObject _current;

	/***
	 * Unity life-cycle
	 *
	 */
	private void OnEnable() => _current = defaultCharacter;

	/**
	* IAvatarPlugin
	*
	*/
	public void Register() {
		_model = avatar.Model;
		SetVariant(_model.Index());
	}

	public void Unregister() {}


	/***
	 * IVisible
	 *
	 */
	public void SetVisibility(bool value) {
		if (_current == null) _current = defaultCharacter;
		_hide = value;
		_current.SetActive(value);
	}

	/**
	 * Feature implementation
	 *
	 */
	private void SetVariant(int variantIndex) {
		if (!_current.TryGetComponent(out AvatarCharacterVariant variant)) return;
		variant.SetVariant(variantIndex);
	}

	/**
	 * Public interface
	 *
	 */
	public GameObject GetCurrent() => _current;
}