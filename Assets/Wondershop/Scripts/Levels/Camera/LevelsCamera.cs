using System.Collections.Generic;
using UnityEngine;

public class LevelsCamera : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] [Range(0, 10)] private float startSpeed = 1;
    [SerializeField] [Range(0, 10)] private float endSpeed = 1;
    [SerializeField] [Range(0, 10)] private int limiterBuffer = 3;
    [SerializeField] [Range(1, 100)] private float colliderHeight = 50;
    [SerializeField] private List<Vector3> offsets;
    [SerializeField] private Transform mainCameraTransform;
    [SerializeField] private float startDelay;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private ScriptableEventForColliders onTrigger;
    [SerializeField] private ScriptableEventForGameObject onLevelEnd;

    private float _speedProgress;
    private float _delayTimer;
    private Camera _mainCamera;
    private GameObject _boundaries;
    private Vector3 _initialPosition;
    private Vector3? _limiterPosition;
    private readonly List<GameObject> _anchors = new List<GameObject>();

    /**
     * Helpers
     * 
     */
    public float GetStartSpeed() => startSpeed;
    public float GetEndSpeed() => endSpeed;
    public float GetCurrentSpeed() => Mathf.Lerp(startSpeed, endSpeed, _speedProgress);


    /**
     * Unity life-cycle
     * 
     */
    private void Awake()
    {
        // get the main camera
        _mainCamera = Camera.main;
        if (_mainCamera == null) return;
        // get the main camera transform
        if (mainCameraTransform == null) mainCameraTransform = _mainCamera.transform;
        // get the initial position
        _initialPosition = mainCameraTransform.position;
        // create container for the colliders
        _boundaries = new GameObject("MainCameraViewportBoundaries");
        _boundaries.transform.SetParent(mainCameraTransform);
        // create collider based on viewport
        SetCameraViewportColliders();
        // set initial start delay
        _delayTimer = startDelay;
    }

    private void Update()
    {
        // possibility to delay start
        _delayTimer -= Time.deltaTime;
        if (0 <= _delayTimer) return;
        // actually start updating cam
        UpdatePosition();
        UpdateRaycast();
        UpdateLimiter();
    }

    private void UpdatePosition() => mainCameraTransform.position += direction.normalized * (Time.deltaTime * GetCurrentSpeed());

    private void UpdateRaycast()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, 1000f, groundLayers)) return;
        onLevelEnd.TriggerEvent(gameObject);
    }

    private void UpdateLimiter()
    {
        if (_limiterPosition == null) return;
        // stops camera after reaching limiter, currently uses viewport center
        Vector3 cameraPosition = GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(0.5f, 0f, 0)));
        // bool xLimit = _limiterPosition?.x + padding < cameraPosition.x;
        // bool yLimit = _limiterPosition?.y + padding < cameraPosition.y;
        // quick and dirty fix, only count for the z coordinate, fix later
        bool zLimit = _limiterPosition?.z + limiterBuffer < cameraPosition.z;
        if (zLimit) enabled = false;
    }

    private static Vector3 GetPointAtHeight(Ray ray, float height = 0f) =>
        ray.origin + (ray.origin.y - height) / -ray.direction.y * ray.direction;

    private void SetCameraViewportColliders()
    {
        // rear left
        Vector3 rl = GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(0, 0, 0))) + GetOffset(0);
        // front left
        Vector3 fl = GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(0, 1, 0))) + GetOffset(1);
        // front right
        Vector3 fr = GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(1, 1, 0))) + GetOffset(2);
        // rear right
        Vector3 rr = GetPointAtHeight(_mainCamera.ViewportPointToRay(new Vector3(1, 0, 0))) + GetOffset(3);
        // set the walls for each corner
        SetMeshCollider(rl, fl);
        SetViewportAnchor(rl, fl);
        SetMeshCollider(fl, fr);
        SetViewportAnchor(fl, fr);
        SetMeshCollider(fr, rr);
        SetViewportAnchor(fr, rr);
        SetMeshCollider(rr, rl);
        SetViewportAnchor(rr, rl);
        // add back wall trigger
        SetMeshCollider(rl + GetOffset(0) / 2, rr + GetOffset(3) / 2, true);
    }

    private void SetMeshCollider(Vector3 prev, Vector3 next, bool isTrigger = false)
    {
        // create new object for collider
        GameObject viewportCollider = new GameObject("ViewportCollider");
        viewportCollider.transform.SetParent(_boundaries.transform);
        // set the height of the wall (from zero)
        Vector3 height = new Vector3(0, colliderHeight, 0);
        // create required vertices for the plane
        Vector3[] vertices = { prev, prev + height, next, next + height };
        // create required triangles for the plane
        int[] triangles = { 0, 1, 2, 1, 2, 3 };
        // create the actual mesh for the wall
        Mesh mesh = new Mesh { vertices = vertices, triangles = triangles };
        // add MeshCollider to the wall object
        MeshCollider meshCollider = viewportCollider.AddComponent<MeshCollider>();
        // add the generated mesh as the collider
        meshCollider.name = "RuntimeMesh";
        meshCollider.sharedMesh = mesh;
        // check if trigger
        if (!isTrigger) return;
        meshCollider.convex = true;
        // set trigger event if exists
        meshCollider.isTrigger = true;
        EventTriggerEnter trigger = viewportCollider.AddComponent<EventTriggerEnter>();
        trigger.SetOnTriggerEnter(onTrigger);
    }

    private void SetViewportAnchor(Vector3 prev, Vector3 next)
    {
        // adds GameObject anchor between given points
        Vector3 midpoint = prev + (next - prev) * 0.5f;
        _anchors.Add(new GameObject("Anchor") { transform = { parent = _boundaries.transform, position = midpoint } });
    }

    private Vector3 GetOffset(int index) => offsets.Count - 1 < index ? Vector3.zero : offsets[index];

    public GameObject GetAnchor(int index) => _anchors.Count == 0 ? null : _anchors[index];

    public void SetCameraLimit(Vector3 limiter) => _limiterPosition = limiter;

    public void Reset()
    {
        // reset camera position to initial
        mainCameraTransform.position = _initialPosition;
        // reset delay timer
        _delayTimer = startDelay;
        // reset maximum position
        _limiterPosition = null;
    }

    public void UpdateSpeedProgress(float value)
    {
        _speedProgress = value;
    }
}