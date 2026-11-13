using UnityEngine;

public abstract class AView : MonoBehaviour
{
    public float weight = 1f;

    public void SetActive(bool isActive)
    {
        if (CameraController.Instance == null)
        {
            return;
        }

        if (isActive)
        {
            CameraController.Instance.AddView(this);
        }
        else
        {
            CameraController.Instance.RemoveView(this);
        }
    }

    public abstract CameraConfiguration GetConfiguration();
}