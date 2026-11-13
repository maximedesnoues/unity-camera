using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public Camera camera;
    public float smoothingSpeed = 5f;

    private readonly List<AView> activeViews = new List<AView>();

    private CameraConfiguration currentConfiguration;
    private CameraConfiguration targetConfiguration;

    private ACameraModifier[] cameraModifiers;

    private bool isCutRequested;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        cameraModifiers = GetComponents<ACameraModifier>();
    }

    private void Start()
    {
        if (activeViews.Count == 0)
        {
            return;
        }

        targetConfiguration = ComputeAverage();
        currentConfiguration = targetConfiguration;

        CameraConfiguration finalConfiguration = ApplyModifiers(currentConfiguration);

        ApplyConfiguration(finalConfiguration);
    }

    private void Update()
    {
        if (activeViews.Count == 0)
        {
            return;
        }

        targetConfiguration = ComputeAverage();

        if (isCutRequested)
        {
            currentConfiguration = targetConfiguration;
            isCutRequested = false;
        }
        else
        {
            SmoothConfiguration();
        }

        CameraConfiguration finalConfiguration = ApplyModifiers(currentConfiguration);

        ApplyConfiguration(finalConfiguration);
    }

    public void AddView(AView view)
    {
        if (view == null || activeViews.Contains(view))
        {
            return;
        }

        activeViews.Add(view);
    }

    public void RemoveView(AView view)
    {
        if (view == null)
        {
            return;
        }

        activeViews.Remove(view);
    }

    public void Cut()
    {
        isCutRequested = true;
    }

    private CameraConfiguration ComputeAverage()
    {
        CameraConfiguration resultConfiguration = new CameraConfiguration();

        if (activeViews.Count == 0)
        {
            return resultConfiguration;
        }

        float totalWeight = 0f;

        foreach (AView view in activeViews)
        {
            totalWeight += view.weight;
        }

        if (totalWeight <= 0f)
        {
            return resultConfiguration;
        }

        foreach (AView view in activeViews)
        {
            CameraConfiguration configuration = view.GetConfiguration();
            float normalizedWeight = view.weight / totalWeight;

            resultConfiguration.pitch += configuration.pitch * normalizedWeight;
            resultConfiguration.roll += configuration.roll * normalizedWeight;

            resultConfiguration.pivot += configuration.pivot * normalizedWeight;

            resultConfiguration.distance += configuration.distance * normalizedWeight;
            resultConfiguration.fov += configuration.fov * normalizedWeight;
        }

        resultConfiguration.yaw = ComputeAverageYaw();

        return resultConfiguration;
    }

    private float ComputeAverageYaw()
    {
        Vector2 sum = Vector2.zero;

        foreach (AView view in activeViews)
        {
            CameraConfiguration configuration = view.GetConfiguration();
            float angle = configuration.yaw * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            sum += direction * view.weight;
        }

        if (sum.sqrMagnitude < 0.0001f)
        {
            return 0f;
        }

        return Vector2.SignedAngle(Vector2.right, sum);
    }

    private void SmoothConfiguration()
    {
        float t = Mathf.Clamp01(smoothingSpeed * Time.deltaTime);

        currentConfiguration.yaw = Mathf.LerpAngle(currentConfiguration.yaw, targetConfiguration.yaw, t);
        currentConfiguration.pitch = Mathf.Lerp(currentConfiguration.pitch, targetConfiguration.pitch, t);
        currentConfiguration.roll = Mathf.LerpAngle(currentConfiguration.roll, targetConfiguration.roll, t);

        currentConfiguration.pivot = Vector3.Lerp(currentConfiguration.pivot, targetConfiguration.pivot, t);

        currentConfiguration.distance = Mathf.Lerp(currentConfiguration.distance, targetConfiguration.distance, t);
        currentConfiguration.fov = Mathf.Lerp(currentConfiguration.fov, targetConfiguration.fov, t);
    }

    private CameraConfiguration ApplyModifiers(CameraConfiguration configuration)
    {
        foreach (ACameraModifier cameraModifier in cameraModifiers)
        {
            configuration = cameraModifier.Apply(configuration);
        }

        return configuration;
    }

    private void ApplyConfiguration(CameraConfiguration configuration)
    {
        if (camera == null)
        {
            return;
        }

        camera.transform.position = configuration.GetPosition();
        camera.transform.rotation = configuration.GetRotation();
        camera.fieldOfView = configuration.fov;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        currentConfiguration.DrawGizmos(Color.green);
    }
}