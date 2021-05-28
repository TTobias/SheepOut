using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SheepAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minDistanceToWayPoint = 1.2f;
    [SerializeField] float viewDistance = 5;
    [Tooltip("If player moves too close to enemy fov ignored!")]
    [SerializeField] float awarenessDistance = 1;
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
    LayerMask wallLayer;

    Transform enemyUI;
    Image alertFill;
    Camera cam;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wallLayer = LayerMask.NameToLayer("Wall");
        enemyUI = transform.Find("EnemyUI");
        alertFill = enemyUI.Find("Fill").GetComponent<Image>();
        enemyUI.gameObject.SetActive(false);
        cam = Camera.main;

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
        Vector3 pos = SheepTarget.instance.Position;
        Vector3 dir = (pos - transform.position).normalized;
        float dis = Vector3.Distance(transform.position, pos);
        float angle = Vector3.Angle(transform.forward, dir);

        //Wall blocks view
        bool viewBlocked = Physics.Raycast(transform.position, pos, viewDistance, wallLayer);

        if (curState != State.SEE_PLAYER)
        {
            if (viewBlocked) return;
            if ((dis < viewDistance && angle < halfFOV) || dis < awarenessDistance)
            {
                curState = State.SEE_PLAYER;
                seeTimer = SheepTarget.instance.stealthTimer;
            }
        }
        else
        {
            if (dis > viewDistance * 1.2f || angle > halfFOV * 1.1f || viewBlocked)
            {
                curState = State.MOVE;
                enemyUI.gameObject.SetActive(false);
            }
        }
    }

    void PlayerInView()
    {
        enemyUI.gameObject.SetActive(true);
        enemyUI.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

        float stealthDuration = SheepTarget.instance.stealthTimer;
        seeTimer -= Time.deltaTime;
        alertFill.fillAmount = ((stealthDuration - seeTimer) / stealthDuration);
        Vector3 sheepPos = SheepTarget.instance.Position;
        Vector3 normalizedPos = new Vector3(sheepPos.x, transform.position.y, sheepPos.z);
        transform.LookAt(normalizedPos, Vector3.up);
        if(seeTimer < 0)
        {
            //TODO Game Over Handling
            alertFill.color = Color.red;
            Debug.Log("GAME OVER!");
        } 
        else
        {
            alertFill.color = new Color(255, 154, 0);
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

        if (wayPoints.Count <= 1 || wayPoints[0] == null) return;
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
