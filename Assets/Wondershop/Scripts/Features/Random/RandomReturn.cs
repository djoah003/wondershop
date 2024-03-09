using UnityEngine;

public class RandomReturn : MonoBehaviour
{
    private bool _orphan;
    private Transform _originalParent;
    private Vector3 _originalPosition;

    protected virtual void Start()
    {
        _originalParent = transform.parent;
        _originalPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (_orphan) ReturnHome();
    }

    protected virtual void OnTransformParentChanged()
    {
        _orphan = transform.parent == null;
    }

    protected virtual void ReturnHome()
    {
        ReturnToOriginal();
    }

    protected virtual void ReturnToOriginal()
    {
        transform.SetParent(_originalParent);
        transform.position = _originalPosition;
    }
}