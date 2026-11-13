using UnityEngine;

public class Rail : MonoBehaviour
{
    public bool isLoop;

    private float length;

    private void Start()
    {
        ComputeLength();
    }

    public float GetLength()
    {
        return length;
    }

    public Vector3 GetPosition(float distance)
    {
        int nodeCount = transform.childCount;

        if (nodeCount == 0)
        {
            return transform.position;
        }

        if (nodeCount == 1)
        {
            return transform.GetChild(0).position;
        }

        if (length <= 0f)
        {
            ComputeLength();
        }

        if (isLoop)
        {
            distance = Mathf.Repeat(distance, length);
        }
        else
        {
            distance = Mathf.Clamp(distance, 0f, length);
        }

        int segmentCount = isLoop ? nodeCount : nodeCount - 1;

        float remainingDistance = distance;

        for (int segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
        {
            int nextNodeIndex = (segmentIndex + 1) % nodeCount;

            Vector3 segmentStart = transform.GetChild(segmentIndex).position;
            Vector3 segmentEnd = transform.GetChild(nextNodeIndex).position;

            float segmentLength = Vector3.Distance(segmentStart, segmentEnd);

            if (remainingDistance <= segmentLength)
            {
                if (segmentLength <= Mathf.Epsilon)
                {
                    return segmentStart;
                }

                float t = remainingDistance / segmentLength;

                return Vector3.Lerp(segmentStart, segmentEnd, t);
            }

            remainingDistance -= segmentLength;
        }

        return transform.GetChild(nodeCount - 1).position;
    }

    public float GetNearestDistanceOnRail(Vector3 target)
    {
        int nodeCount = transform.childCount;

        if (nodeCount < 2)
        {
            return 0f;
        }

        int segmentCount = isLoop ? nodeCount : nodeCount - 1;

        float shortestDistanceToTarget = float.MaxValue;
        float nearestDistanceOnRail = 0f;
        float accumulatedDistance = 0f;

        for (int segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
        {
            int nextNodeIndex = (segmentIndex + 1) % nodeCount;

            Vector3 segmentStart = transform.GetChild(segmentIndex).position;
            Vector3 segmentEnd = transform.GetChild(nextNodeIndex).position;

            Vector3 nearestPointOnSegment = MathUtils.GetNearestPointOnSegment(segmentStart, segmentEnd, target);
            float distanceToTarget = Vector3.Distance(nearestPointOnSegment, target);

            if (distanceToTarget < shortestDistanceToTarget)
            {
                shortestDistanceToTarget = distanceToTarget;
                nearestDistanceOnRail = accumulatedDistance + Vector3.Distance(segmentStart, nearestPointOnSegment);
            }

            accumulatedDistance += Vector3.Distance(segmentStart, segmentEnd);
        }

        return nearestDistanceOnRail;
    }

    private void ComputeLength()
    {
        length = 0f;

        int nodeCount = transform.childCount;

        for (int segmentIndex = 0; segmentIndex < nodeCount - 1; segmentIndex++)
        {
            Vector3 segmentStart = transform.GetChild(segmentIndex).position;
            Vector3 segmentEnd = transform.GetChild(segmentIndex + 1).position;
            length += Vector3.Distance(segmentStart, segmentEnd);
        }

        if (isLoop && nodeCount > 1)
        {
            Vector3 lastNode = transform.GetChild(nodeCount - 1).position;
            Vector3 firstNode = transform.GetChild(0).position;
            length += Vector3.Distance(lastNode, firstNode);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        int nodeCount = transform.childCount;

        for (int segmentIndex = 0; segmentIndex < nodeCount - 1; segmentIndex++)
        {
            Vector3 segmentStart = transform.GetChild(segmentIndex).position;
            Vector3 segmentEnd = transform.GetChild(segmentIndex + 1).position;
            Gizmos.DrawLine(segmentStart, segmentEnd);
        }

        if (isLoop && nodeCount > 1)
        {
            Vector3 lastNode = transform.GetChild(nodeCount - 1).position;
            Vector3 firstNode = transform.GetChild(0).position;
            Gizmos.DrawLine(lastNode, firstNode);
        }
    }
}