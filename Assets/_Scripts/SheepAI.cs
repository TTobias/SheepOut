using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SheepAI : MonoBehaviour
{
    [SerializeField] List<WayPoint> wayPoints = new List<WayPoint>();
    [SerializeField] float minDistanceToWayPoint = 1.2f;
    NavMeshAgent agent;
    int curWayPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        MoveLogic();
    }

    void MoveLogic()
    {
        if (wayPoints.Count == 0)
        {
            Debug.LogWarning($"{name} does not have waypoints!");
            return;
        }

        Vector3 target = wayPoints[curWayPoint].Position;

        agent.destination = target;
        if(Vector2.Distance(transform.position, target) < minDistanceToWayPoint)
        {
            curWayPoint++;
            if (curWayPoint >= wayPoints.Count) curWayPoint = 0;
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject != gameObject) return;
#endif

        if (wayPoints.Count <= 1) return;
        Vector3 prevPos = wayPoints[0].Position;
        for(int i = 1; i < wayPoints.Count; i++)
        {
            WayPoint w = wayPoints[i];
            if (w == null) continue;
            Vector3 nextPos = w.Position;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(prevPos, nextPos);
            prevPos = nextPos;
        }
    }
}
