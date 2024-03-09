using UnityEngine;

public struct ColliderEventArgs
{
    public GameObject That;
    public GameObject Other;
    public Vector3 Normal;
    public Vector3 Position;

    public ColliderEventArgs(GameObject that, GameObject other)
    {
        Position = Vector3.zero;
        Normal = Vector3.zero;
        That = that;
        Other = other;
    }
}