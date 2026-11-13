using UnityEngine;

public abstract class ACameraModifier : MonoBehaviour
{
    public abstract CameraConfiguration Apply(CameraConfiguration configuration);
}