using UnityEngine;
using System.Collections.Generic;

public class WaypointPath : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    /// <summary>
    /// </summary>
    public float GetTotalLength()
    {
        float total = 0f;
        for (int i = 0; i < waypoints.Count - 1; i++)
            total += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        return total;
    }

    /// <summary>
    /// </summary>
    public float GetProgress(Vector3 position, int currentWaypointIndex)
    {
        float totalLength = GetTotalLength();
        if (totalLength <= 0f) return 0f;

        float covered = 0f;
        for (int i = 0; i < currentWaypointIndex && i < waypoints.Count - 1; i++)
            covered += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);

        if (currentWaypointIndex < waypoints.Count)
        {
            int prevIndex = Mathf.Max(0, currentWaypointIndex - 1);
            covered += Vector3.Distance(waypoints[prevIndex].position, position);
        }

        return Mathf.Clamp01(covered / totalLength);
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null) continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            Gizmos.DrawSphere(waypoints[i].position, 0.15f);
        }
        Gizmos.DrawSphere(waypoints[waypoints.Count - 1].position, 0.15f);
    }
}
