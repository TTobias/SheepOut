using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    public List<WayPoint> wayPoints = new List<WayPoint>();

    [ContextMenu("Update WayPoints")]
    void UpdateWayPoints()
    {
        wayPoints = new List<WayPoint>(GetComponentsInChildren<WayPoint>());
    }
    public void OnDrawGizmos()
    {
        if (wayPoints.Count <= 1 || wayPoints[0] == null) return;
        Vector3 first = wayPoints[0].Position;
        Vector3 prevPos = first;
        for (int i = 1; i < wayPoints.Count; i++)
        {
            WayPoint w = wayPoints[i];
            if (w == null) continue;
            Vector3 nextPos = w.Position;
            Gizmos.color = Color.blue;
            DrawArrow(prevPos, nextPos, Color.blue);
            prevPos = nextPos;
        }
        DrawArrow(first, prevPos, Color.blue);
    }

    public static void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        Gizmos.color = color;
        Vector3 direction = (end - start).normalized;
        Vector3 right = Quaternion.LookRotation(direction) * 
            Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * 
            Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawLine(start + Vector3.up * 0.1f, end + Vector3.up * 0.1f);
        Gizmos.DrawRay(end + Vector3.up * 0.1f, right * 0.4f + Vector3.up * 0.1f);
        Gizmos.DrawRay(end + Vector3.up * 0.1f, left * 0.4f + Vector3.up * 0.1f);
    }
}
