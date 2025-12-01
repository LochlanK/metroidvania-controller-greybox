using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;      // Object to follow
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector2 offset;        // Optional offset

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        // Target position but ignore Z axis
        Vector3 targetPos = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z // keep camera Z unchanged
        );

        // Smooth follow
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
