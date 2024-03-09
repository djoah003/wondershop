using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public interface IAvatar
{
	public IModel Model { get; }
	public void Setup(Action onSetupDone);
	public void Inject(GameObject blueprint);
	public void Deject(GameObject target);
	public void DejectAll();
	public IInput Input();
	public GameObject GameObject();
	public Transform Transform();
}

public interface IAvatarPlugin
{
	public void Register();
	public void Unregister();
}

public interface IVisible
{
	public void SetVisibility(bool value);
}

public abstract class AvatarBehaviour : MonoBehaviour
{
	public bool active = true;
	public virtual void SetActive(bool value) => active = value;
	protected Avatar avatar => transform.parent.GetComponent<Avatar>();
}


/// <summary>
/// Avatar class handles the main update cycle for any input and behaviour related to Avatar. It acts as a bridge
/// between the Avatar's internal implementation and external world. 
/// </summary>
public class Avatar : MonoBehaviour, IAvatar
{
	// custom life-cycle updates
	public event Action OnTick;
	public event Action OnLateTick;
	public event Action OnFixedTick;

	// avatar input pipeline
	public event Action OnPush;
	public event Action<bool> OnHold;
	public event Action<Vector2> OnMove;

	// local dependencies
	private bool _setup;

	// injected dependencies
	private readonly List<GameObject> _injected = new List<GameObject>();
	private IInput input { get; set; }
	public IModel Model { get; private set; }
	public ICamera mainCamera { get; private set; }


	/**
	 * IAvatar
	 *
	 */
	public void Setup(Action onSetupDone) {
		if (_setup) return;
		_setup = true;
		// setup handles connecting the input for Avatar
		input = GetComponent<IInput>();
		input.push += () => OnPush?.Invoke();
		input.hold += hold => OnHold?.Invoke(hold);
		input.move += move => OnMove?.Invoke(move);
		// call model sync to request Avatar data
		Model = GetComponent<IModel>();
		if (Model != null) Model.Sync(GetComponent<PlayerInput>(), onSetupDone);
		else onSetupDone?.Invoke();
		// call ICamera to cache the current camera
		mainCamera = GetComponent<ICamera>();
		mainCamera.TryGetCamera(out Camera _);
		// Try to register components, mainly for development purposes, normally plugins are injected
		StartCoroutine(Register());
	}

	/**
	 * Inject & Dejecting GameObjects
	 *
	 */
	public void Inject(GameObject blueprint) {
		// clean all the destroyed references
		_injected.RemoveAll(injection => injection.Equals(null));
		// only one dependency of type allowed
		if (_injected.Exists(obj => obj.name.StartsWith(blueprint.name))) return;
		// unregister current components
		Unregister();
		// actually inject the dependency
		_injected.Add(Instantiate(blueprint, transform.GetChild(0)));
		// finally, register all the plugins
		StartCoroutine(Register());
	}

	public void Deject(GameObject target) {
		// clean all the destroyed references
		_injected.RemoveAll(injection => injection.Equals(null));
		// only deject if target found
		if (!_injected.Exists(obj => obj.name.StartsWith(target.name))) return;
		// unregister current components
		Unregister();
		// actually destroy the target plugin
		for (int i = 0; i < _injected.Count; i++) {
			if (!_injected[i].name.StartsWith(target.name)) continue;
			Destroy(_injected[i]);
		}
		// finally, register all the plugins
		StartCoroutine(Register());
	}

	public void DejectAll() {
		Unregister();
		for (int i = 0; i < _injected.Count; i++) Destroy(_injected[i]);
		_injected.Clear();
		StartCoroutine(Register());
	}

	public IInput Input() => input;
	public Transform Transform() => transform.GetChild(0);
	public GameObject GameObject() => transform.GetChild(0).gameObject;

	/**
	 * Internal
	 *
	 */
	private void Unregister() {
		foreach (IAvatarPlugin plugin in GetComponentsInChildren<IAvatarPlugin>()) plugin.Unregister();
	}

	private IEnumerator Register() {
		yield return new WaitForSeconds(0);
		foreach (IAvatarPlugin plugin in GetComponentsInChildren<IAvatarPlugin>()) plugin.Register();
	}

	/**
	 * Unity life-cycle
	 *
	 */
	private void Update() => OnTick?.Invoke();
	private void LateUpdate() => OnLateTick?.Invoke();
	private void FixedUpdate() => OnFixedTick?.Invoke();

	/**
	 * Input events
	 *
	 */
	[UsedImplicitly]
	public void OnDeviceLost(PlayerInput player) => Invoke(nameof(Remove), 0f);

	private void Remove() => Destroy(gameObject);
}