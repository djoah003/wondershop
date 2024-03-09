using Cinemachine;
using UnityEngine;

public class LevelsCameraBounds : MonoBehaviour
{
    [SerializeField] [Range(1, 150)] private float colliderSize = 50;
    [SerializeField] private float leftOffset;
    [SerializeField] private float topOffset;
    [SerializeField] private float rightOffset;
    [SerializeField] private float bottomOffset;

    private Camera _mainCamera;
    private GameObject _boundaries;
    private CinemachineBrain _brain;
    private readonly Transform[] _colliders = new Transform[4];
    private Transform _mainCameraTransform;

    /**
     * Unity life-cycle
     * 
     */
    public void Initialize()
    {
        // get the main camera
        _mainCamera = Camera.main;
        if (_mainCamera == null) return;
        // get the main camera transform
        _mainCameraTransform = _mainCamera.transform;
        _brain = _mainCameraTransform.GetComponent<CinemachineBrain>();
        // create collider based on viewport
        CreateColliders();
    }

    private void OnEnable() => MoveColliders();

    private void Update()
    {
        if (_brain && _brain.IsBlending) MoveColliders();
    }

    /**
     * Camera bounds
     * 
     */
    private static Vector3 GetPointAtHeight(Ray ray) =>
        ray.origin + ray.origin.y / -ray.direction.y * ray.direction;

    private Vector3 TopLeft() =>
        GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(0, 1, 0))) +
        new Vector3(leftOffset, 0, topOffset);

    private Vector3 TopRight() =>
        GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(1, 1, 0))) +
        new Vector3(rightOffset, 0, topOffset);

    private Vector3 BotLeft() =>
        GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(0, 0, 0))) +
        new Vector3(leftOffset, 0, bottomOffset);

    private Vector3 BotRight() =>
        GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(1, 0, 0))) +
        new Vector3(rightOffset, 0, bottomOffset);


    private void MoveColliders()
    {
        if (!_boundaries) return;
        _boundaries.transform.localRotation = Quaternion.Inverse(_boundaries.transform.parent.rotation);
        // get positions
        Vector3 topLeft = TopLeft();
        Vector3 topRight = TopRight();
        Vector3 bottomLeft = BotLeft();
        Vector3 bottomRight = BotRight();
        // set the positions
        _colliders[0].position = Vector3.Lerp(bottomLeft, topLeft, 0.5f);
        _colliders[0].right = (bottomLeft - topLeft).normalized;
        _colliders[1].position = Vector3.Lerp(topLeft, topRight, 0.5f);
        _colliders[1].right = (topRight - topLeft).normalized;
        _colliders[2].position = Vector3.Lerp(topRight, bottomRight, 0.5f);
        _colliders[2].right = (topRight - bottomRight).normalized;
        _colliders[3].position = Vector3.Lerp(bottomLeft, bottomRight, 0.5f);
        _colliders[3].right = (bottomRight - bottomLeft).normalized;
    }

    private void CreateColliders()
    {
        // create container for the colliders
        _boundaries = new GameObject("MainCameraViewportBoundaries");
        _boundaries.transform.SetParent(_mainCameraTransform);
        _boundaries.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        // get positions
        Vector3 topLeft = TopLeft();
        Vector3 topRight = TopRight();
        Vector3 bottomLeft = BotLeft();
        Vector3 bottomRight = BotRight();
        // create the colliders
        SetMeshCollider(0, bottomLeft, topLeft, "left");
        SetMeshCollider(1, topLeft, topRight, "top");
        SetMeshCollider(2, topRight, bottomRight, "right");
        SetMeshCollider(3, bottomLeft, bottomRight, "bottom");
    }

    private void SetMeshCollider(int index, Vector3 corner1, Vector3 corner2, string gameObjectName)
    {
        // create new object for collider
        GameObject viewportCollider = new(gameObjectName);
        viewportCollider.transform.SetParent(_boundaries.transform);
        viewportCollider.transform.SetPositionAndRotation(Vector3.Lerp(corner1, corner2, 0.5f), Quaternion.identity);
        viewportCollider.transform.right = (corner2 - corner1).normalized;
        viewportCollider.layer = LayerMask.NameToLayer("Viewport");
        // add box collider
        BoxCollider boxCollider = viewportCollider.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(colliderSize, colliderSize, 1);
        _colliders[index] = viewportCollider.transform;
    }


    public void UseColliders(bool value) => _boundaries.SetActive(value);
}