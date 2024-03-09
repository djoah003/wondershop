using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AvatarCollisions : AvatarBehaviour
{
    [SerializeField] private ScriptableEventForColliders onPlayerCollision;
    [SerializeField] private LayerMask collisionEventLayers;


    /**
     * Event listeners
     * 
     */
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (collisionEventLayers != (collisionEventLayers | (1 << hit.gameObject.layer))) return;
        if (onPlayerCollision) onPlayerCollision.TriggerEvent(new ColliderEventArgs { That = gameObject, Other = hit.gameObject, Normal = hit.normal, Position = hit.point});
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collisionEventLayers != (collisionEventLayers | (1 << other.gameObject.layer))) return;
        if (onPlayerCollision) onPlayerCollision.TriggerEvent(new ColliderEventArgs { That = gameObject, Other = other.gameObject });
    }
}