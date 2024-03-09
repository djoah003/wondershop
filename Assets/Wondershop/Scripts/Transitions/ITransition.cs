using UnityEngine;

public interface ITransition
{
    public GameObject gameObject { get; }
    public float Open();
    public float Close();
    public void SetOpen();
    public void SetClosed();
}