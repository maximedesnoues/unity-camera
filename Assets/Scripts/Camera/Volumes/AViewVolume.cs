using UnityEngine;

public abstract class AViewVolume : MonoBehaviour
{
    public int priority;
    public AView view;
    public bool isCutOnSwitch;

    protected bool IsActive { get; private set; }

    private int uid;
    private static int nextUid;

    public int Uid => uid;

    protected virtual void Awake()
    {
        uid = nextUid;
        nextUid++;
    }

    public virtual float ComputeSelfWeight()
    {
        return 1f;
    }

    protected void SetActive(bool isActive)
    {
        if (IsActive == isActive)
        {
            return;
        }

        if (ViewVolumeBlender.Instance == null)
        {
            return;
        }

        IsActive = isActive;

        if (isActive)
        {
            ViewVolumeBlender.Instance.AddVolume(this);
        }
        else
        {
            ViewVolumeBlender.Instance.RemoveVolume(this);
        }

        if (isCutOnSwitch)
        {
            ViewVolumeBlender.Instance.UpdateBlend();

            if (CameraController.Instance != null)
            {
                CameraController.Instance.Cut();
            }
        }
    }
}