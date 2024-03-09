using UnityEngine;

[RequireComponent(typeof(LevelsCamera))]
public class LevelsCameraSafety : MonoBehaviour
{
    [SerializeField] [Range(0, 10)] private int buffer;
    [SerializeField] private LayerMask groundLayers;
    private GameObject _anchor;
    private LevelsCamera _levelsCamera;

    /**
     * Unity life-cycle
     * 
     */
    private void Start() => _levelsCamera = GetComponent<LevelsCamera>();

    private void FixedUpdate()
    {
        if (_levelsCamera == null) return;
        if (_anchor == null) _anchor = _levelsCamera.GetAnchor(3);
        if (_anchor == null) return;
        foreach (GameObject player in PlayerManager.Players()) UpdatePlayer(player, _anchor.transform.position);
    }

    /**
     * Camera safety
     * 
     */
    private void UpdatePlayer(GameObject player, Vector3 limit)
    {
        Vector3 playerPosition = player.transform.position;
        // if player is not left behind, return early
        if (limit.z - buffer < playerPosition.z) return;
        // find new random position for player
        Vector3 newPosition = limit + Vector3.forward;
        Vector3 testPosition = new(playerPosition.x, 100.0f, newPosition.z);
        // make sure that the point is big enough for player
        float radius = player.GetComponent<CharacterController>().radius;
        player.transform.position =
            Physics.SphereCast(testPosition, radius, Vector3.down, out RaycastHit hit, 110.0f, groundLayers)
                ? new Vector3(playerPosition.x, 100.0f - hit.distance, newPosition.z)
                : new Vector3(playerPosition.x, playerPosition.y, newPosition.z);
    }
}