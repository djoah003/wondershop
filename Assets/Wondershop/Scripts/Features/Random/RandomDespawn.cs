
using System;
using UnityEngine;

public class RandomDespawn : MonoBehaviour
{
    // timers
    private float _despawnTime;
    private float _blinkInterval;
    private bool _blink;

    // position
    private Vector3 _position;
    private bool _cancelOnMove;

    private Action _onDespawn;

    protected void OnDestroy()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(true);
    }

    public void Initialize(float despawnInSeconds, Action onDespawn = null, bool cancelOnMove = false, float blinkInterval = 0.3f)
    {
        _despawnTime = despawnInSeconds;
        _onDespawn = onDespawn;
        _blinkInterval = blinkInterval;
        _cancelOnMove = cancelOnMove;
        _position = transform.position;
    }

    private void Update()
    {
        _despawnTime -= Time.deltaTime;
        if (_despawnTime < 0f) Deactivate();
        if (_cancelOnMove && _position != transform.position) Destroy(this);
        if (_despawnTime < 5f && !IsInvoking(nameof(Blink))) InvokeRepeating(nameof(Blink), 0f, _blinkInterval);
    }

    private void Blink()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(!_blink);
        _blink = !_blink;
    }

    private void Deactivate()
    {
        CancelInvoke(nameof(Blink));
        _onDespawn?.Invoke();
        gameObject.SetActive(false);
        Destroy(this);
    }
}