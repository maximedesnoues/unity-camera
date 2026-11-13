using UnityEngine;

public class ScreenShakeModifier : ACameraModifier
{
    public float shakeSpeed = 20f;

    public float shakeXMax = 0.2f;
    public float shakeYMax = 0.2f;

    public float intensityDecreaseSpeed = 1f;

    private float seedX;
    private float seedY;

    private float intensity;

    private void Awake()
    {
        seedX = Random.Range(0f, 1000f);
        seedY = Random.Range(0f, 1000f);
    }

    public void Shake(float shakeIntensity)
    {
        intensity = Mathf.Clamp01(shakeIntensity);
    }

    public override CameraConfiguration Apply(CameraConfiguration configuration)
    {
        if (intensity <= 0f)
        {
            return configuration;
        }

        intensity = Mathf.Max(intensity - intensityDecreaseSpeed * Time.deltaTime, 0f);

        float t = Time.time * shakeSpeed;

        float noiseX = Mathf.PerlinNoise(seedX + t, 0f) * 2f - 1f;
        float noiseY = Mathf.PerlinNoise(seedY + t, 0f) * 2f - 1f;

        float intensitySquared = intensity * intensity;

        Vector3 shakeOffset = new Vector3(noiseX * shakeXMax * intensitySquared, noiseY * shakeYMax * intensitySquared, 0f);

        configuration.pivot += configuration.GetRotation() * shakeOffset;

        return configuration;
    }
}