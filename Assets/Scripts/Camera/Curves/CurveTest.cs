using UnityEngine;

public class CurveTest : MonoBehaviour
{
    public Curve curve;

    private void OnDrawGizmos()
    {
        if (curve == null)
        {
            return;
        }

        curve.DrawGizmo(Color.cyan, transform.localToWorldMatrix);
    }
}