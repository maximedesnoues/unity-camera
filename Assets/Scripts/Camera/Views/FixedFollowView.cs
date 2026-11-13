using UnityEngine;

public class FixedFollowView : AView
{
    public float roll;
    public float fov = 60f;

    public Transform target;
    public Transform centralPoint;

    public float yawOffsetMax = 45f;
    public float pitchOffsetMax = 30f;

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration configuration = new CameraConfiguration();

        if (target == null)
        {
            return configuration;
        }

        ComputeAnglesTowards(target.position, out float targetYaw, out float targetPitch);

        float yaw = targetYaw;
        float pitch = targetPitch;

        if (centralPoint != null)
        {
            ComputeAnglesTowards(centralPoint.position, out float centralYaw, out float centralPitch);

            float yawDelta = Mathf.DeltaAngle(centralYaw, targetYaw);
            yawDelta = Mathf.Clamp(yawDelta, -yawOffsetMax, yawOffsetMax);

            float pitchDelta = targetPitch - centralPitch;
            pitchDelta = Mathf.Clamp(pitchDelta, -pitchOffsetMax, pitchOffsetMax);

            yaw = centralYaw + yawDelta;
            pitch = centralPitch + pitchDelta;
        }

        configuration.yaw = yaw;
        configuration.pitch = pitch;
        configuration.roll = roll;

        configuration.pivot = transform.position;

        configuration.distance = 0f;
        configuration.fov = fov;

        return configuration;
    }

    private void ComputeAnglesTowards(Vector3 point, out float yaw, out float pitch)
    {
        Vector3 direction = (point - transform.position).normalized;

        yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
    }
}