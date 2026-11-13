using UnityEngine;

public class FixedView : AView
{
    public float yaw;
    public float pitch;
    public float roll;
    public float fov = 60f;

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration configuration = new CameraConfiguration();

        configuration.yaw = yaw;
        configuration.pitch = pitch;
        configuration.roll = roll;

        configuration.pivot = transform.position;

        configuration.distance = 0f;
        configuration.fov = fov;

        return configuration;
    }
}