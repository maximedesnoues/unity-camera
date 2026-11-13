using UnityEngine;

public class SphereViewVolume : AViewVolume
{
    public Transform target;

    public float outerRadius = 10f;
    public float innerRadius = 5f;

    private float distance;

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        innerRadius = Mathf.Min(innerRadius, outerRadius);

        distance = Vector3.Distance(target.position, transform.position);

        if (distance <= outerRadius && !IsActive)
        {
            SetActive(true);
        }
        else if (distance > outerRadius && IsActive)
        {
            SetActive(false);
        }
    }

    public override float ComputeSelfWeight()
    {
        if (innerRadius == outerRadius)
        {
            return distance <= innerRadius ? 1f : 0f;
        }

        float weight = Mathf.InverseLerp(outerRadius, innerRadius, distance);

        return Mathf.Clamp01(weight);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }
}