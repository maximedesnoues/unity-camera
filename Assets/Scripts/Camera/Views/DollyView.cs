using UnityEngine;

public class DollyView : AView
{
    public float roll;
    public float distance;
    public float fov = 60f;

    public Transform target;
    public Rail rail;

    public float distanceOnRail;
    public float speed = 5f;

    public bool isAuto;

    private void Update()
    {
        if (rail == null)
        {
            return;
        }

        if (isAuto)
        {
            if (target == null)
            {
                return;
            }

            distanceOnRail = rail.GetNearestDistanceOnRail(target.position);
        }
        else
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            distanceOnRail += horizontalInput * speed * Time.deltaTime;

            if (rail.isLoop)
            {
                distanceOnRail = Mathf.Repeat(distanceOnRail, rail.GetLength());
            }
            else
            {
                distanceOnRail = Mathf.Clamp(distanceOnRail, 0f, rail.GetLength());
            }
        }
    }

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration configuration = new CameraConfiguration();

        if (rail == null || target == null)
        {
            return configuration;
        }

        Vector3 railPosition = rail.GetPosition(distanceOnRail);
        Vector3 directionToTarget = (target.position - railPosition).normalized;

        float yaw = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(directionToTarget.y) * Mathf.Rad2Deg;

        configuration.yaw = yaw;
        configuration.pitch = pitch;
        configuration.roll = roll;

        configuration.pivot = railPosition;

        configuration.distance = distance;
        configuration.fov = fov;

        return configuration;
    }
}