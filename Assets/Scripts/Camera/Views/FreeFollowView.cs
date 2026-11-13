using UnityEngine;

public class FreeFollowView : AView
{
    [Header("Bottom / Middle / Top")]
    public float[] pitch = new float[3];
    public float[] roll = new float[3];
    public float[] fov = new float[3];

    [Header("Yaw")]
    public float yaw;
    public float yawSpeed = 90f;

    [Header("Target")]
    public Transform target;

    [Header("Curve")]
    public Curve curve;

    [Range(0f, 1f)]
    public float curvePosition = 0.5f;

    public float curveSpeed = 0.5f;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        yaw += horizontalInput * yawSpeed * Time.deltaTime;
        curvePosition += verticalInput * curveSpeed * Time.deltaTime;

        curvePosition = Mathf.Clamp01(curvePosition);
    }

    public override CameraConfiguration GetConfiguration()
    {
        CameraConfiguration configuration = new CameraConfiguration();

        if (target == null || curve == null)
        {
            return configuration;
        }

        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
        Matrix4x4 curveToWorldMatrix = Matrix4x4.TRS(target.position, yawRotation, Vector3.one);

        Vector3 cameraPosition = curve.GetPosition(curvePosition, curveToWorldMatrix);

        float interpolatedPitch = InterpolateThreeValues(pitch[0], pitch[1], pitch[2], curvePosition);
        float interpolatedRoll = InterpolateThreeValues(roll[0], roll[1], roll[2], curvePosition);
        float interpolatedFov = InterpolateThreeValues(fov[0], fov[1], fov[2], curvePosition);

        configuration.yaw = yaw;
        configuration.pitch = interpolatedPitch;
        configuration.roll = interpolatedRoll;

        configuration.pivot = cameraPosition;

        configuration.distance = 0f;
        configuration.fov = interpolatedFov;

        return configuration;
    }

    private float InterpolateThreeValues(float bottom, float middle, float top, float t)
    {
        if (t <= 0.5f)
        {
            float localT = t / 0.5f;
            return Mathf.Lerp(bottom, middle, localT);
        }
        else
        {
            float localT = (t - 0.5f) / 0.5f;
            return Mathf.Lerp(middle, top, localT);
        }
    }
}