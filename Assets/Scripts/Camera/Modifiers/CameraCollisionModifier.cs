using UnityEngine;

public class CameraCollisionModifier : ACameraModifier
{
    public Transform target;

    public LayerMask collisionLayers = ~0;
    public float collisionOffset = 0.2f;

    public override CameraConfiguration Apply(CameraConfiguration configuration)
    {
        if (target == null)
        {
            return configuration;
        }

        Vector3 cameraPosition = configuration.GetPosition();
        Vector3 directionToCamera = cameraPosition - target.position;
        float distanceToCamera = directionToCamera.magnitude;

        if (distanceToCamera <= Mathf.Epsilon)
        {
            return configuration;
        }

        directionToCamera /= distanceToCamera;

        if (Physics.Raycast(target.position, directionToCamera, out RaycastHit hit, distanceToCamera, collisionLayers))
        {
            float correctedDistanceToCamera = Mathf.Max(hit.distance - collisionOffset, 0f);
            Vector3 correctedCameraPosition = target.position + directionToCamera * correctedDistanceToCamera;

            configuration.pivot = correctedCameraPosition;
            configuration.distance = 0f;
        }

        return configuration;
    }
}