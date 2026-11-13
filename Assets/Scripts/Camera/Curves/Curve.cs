using UnityEngine;

[System.Serializable]
public class Curve
{
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public Vector3 D;

    public Vector3 GetPosition(float t)
    {
        return MathUtils.CubicBezier(A, B, C, D, t);
    }

    public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix)
    {
        Vector3 localPosition = GetPosition(t);
        return localToWorldMatrix.MultiplyPoint(localPosition);
    }

    public void DrawGizmo(Color color, Matrix4x4 localToWorldMatrix)
    {
        Gizmos.color = color;

        Vector3 worldA = localToWorldMatrix.MultiplyPoint(A);
        Vector3 worldB = localToWorldMatrix.MultiplyPoint(B);
        Vector3 worldC = localToWorldMatrix.MultiplyPoint(C);
        Vector3 worldD = localToWorldMatrix.MultiplyPoint(D);

        Gizmos.DrawSphere(worldA, 0.1f);
        Gizmos.DrawSphere(worldB, 0.1f);
        Gizmos.DrawSphere(worldC, 0.1f);
        Gizmos.DrawSphere(worldD, 0.1f);

        const int steps = 30;

        Vector3 previousPosition = worldA;

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 currentPosition = GetPosition(t, localToWorldMatrix);

            Gizmos.DrawLine(previousPosition, currentPosition);

            previousPosition = currentPosition;
        }
    }
}