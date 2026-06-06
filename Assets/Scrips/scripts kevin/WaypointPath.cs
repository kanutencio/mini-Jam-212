using UnityEngine;
using System.Collections.Generic;

public class WaypointPath : MonoBehaviour
{
    [Header("Waypoints")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Gizmos")]
    public Color pathColor = Color.yellow;
    public float waypointRadius = 0.3f;

    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Count) return null;
        return waypoints[index];
    }

    public int WaypointCount => waypoints.Count;

    public float GetTotalLength()
    {
        float total = 0f;
        for (int i = 0; i < waypoints.Count - 1; i++)
            total += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        return total;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = pathColor;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null) continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.color = (i == 0) ? Color.green : (i == waypoints.Count - 1 ? Color.red : pathColor);
            Gizmos.DrawSphere(waypoints[i].position, waypointRadius);
        }
    }
}