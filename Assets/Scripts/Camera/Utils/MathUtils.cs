using UnityEngine;

public static class MathUtils
{
    public static Vector3 GetNearestPointOnSegment(Vector3 a, Vector3 b, Vector3 target)
    {
        Vector3 ab = b - a;
        float length = ab.magnitude;

        if (length <= Mathf.Epsilon)
        {
            return a;
        }

        Vector3 direction = ab / length;
        Vector3 ac = target - a;

        float dot = Vector3.Dot(ac, direction);
        dot = Mathf.Clamp(dot, 0f, length);

        return a + direction * dot;
    }

    public static Vector3 LinearBezier(Vector3 A, Vector3 B, float t)
    {
        return Vector3.Lerp(A, B, t);
    }

    public static Vector3 QuadraticBezier(Vector3 A, Vector3 B, Vector3 C, float t)
    {
        Vector3 AB = LinearBezier(A, B, t);
        Vector3 BC = LinearBezier(B, C, t);
        return LinearBezier(AB, BC, t);
    }

    public static Vector3 CubicBezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        Vector3 ABC = QuadraticBezier(A, B, C, t);
        Vector3 BCD = QuadraticBezier(B, C, D, t);
        return LinearBezier(ABC, BCD, t);
    }
}