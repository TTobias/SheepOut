using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SheepAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minDistanceToWayPoint = 1.2f;
    [SerializeField] float viewDistance = 5;
    [SerializeField] float halfFOV = 60;
    [Tooltip("How long does the sheep wait after reaching a checkpoint")]
    [SerializeField] float waitTime = 2.0f;
    [Header("WayPoints")]
    [SerializeField] List<WayPoint> wayPoints = new List<WayPoint>();
    NavMeshAgent agent;
    int curWayPoint = 0;

    State curState = State.MOVE;
    float waitTimer = 0;
    float seeTimer = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        LookForPlayer();
        switch (curState)
        {
            case State.MOVE:
                MoveLogic();
                break;
            case State.WAIT_AT_CHECKPOINT:
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0) curState = State.MOVE;
                break;
            case State.SEE_PLAYER:
                PlayerInView();
                break;
            default:
                break;
        }
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
        if (Vector2.Distance(transform.position, target) < minDistanceToWayPoint)
        {
            curWayPoint++;
            if (curWayPoint >= wayPoints.Count) curWayPoint = 0;
            curState = State.WAIT_AT_CHECKPOINT;
            waitTimer = waitTime;
        }
    }

    void LookForPlayer()
    {
        Vector3 pos = DummyPlayer.instance.Position;
        Vector3 dir = (pos - transform.position).normalized;
        float dis = Vector3.Distance(transform.position, pos);
        float angle = Vector3.Angle(transform.forward, dir);

        //Wall blocks view
        if (Physics.Raycast(transform.position, pos, viewDistance, LayerMask.NameToLayer("Wall")))
            return;

        if (curState != State.SEE_PLAYER)
        {
            if (dis < viewDistance && angle < halfFOV)
            {
                curState = State.SEE_PLAYER;
                seeTimer = DummyPlayer.instance.stealthTimer;
            }
        }
        else
        {
            if (dis > viewDistance * 1.2f || angle > halfFOV * 1.1f)
            {
                curState = State.MOVE;
            }
        }
    }

    void PlayerInView()
    {
        seeTimer -= Time.deltaTime;
        transform.LookAt(DummyPlayer.instance.transform, Vector3.up);
        if(seeTimer < 0)
        {
            Debug.Log("GAME OVER!");
        }
    }

    public enum State
    {
        MOVE, WAIT_AT_CHECKPOINT, SEE_PLAYER
    }



    private void OnDrawGizmosSelected()
    {
        //Vision Cone
        Gizmos.color = Color.green;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * viewDistance);
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        Gizmos.DrawRay(transform.position, rightRayDirection * viewDistance);

        if (wayPoints.Count <= 1) return;
        Vector3 prevPos = wayPoints[0].Position;
        for (int i = 1; i < wayPoints.Count; i++)
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
