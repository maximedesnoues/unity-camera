using UnityEngine;

public class TriggeredViewVolume : AViewVolume
{
    public GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if (target != null && other.gameObject == target)
        {
            SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (target != null && other.gameObject == target)
        {
            SetActive(false);
        }
    }
}