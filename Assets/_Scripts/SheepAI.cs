using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SheepAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minDistanceToWayPoint = 0.2f;
    [SerializeField] float viewDistance = 20;
    [Tooltip("influences how fast sheep sees player")]
    [SerializeField] float awarenessLevel = 1.0f;
    [Tooltip("If player moves too close to enemy fov ignored!")]
    [SerializeField] float awarenessDistance = 1.8f;
    [SerializeField] float halfFOV = 60;

    [Header("WayPoints")]
    [SerializeField] WayPointPath path;
    NavMeshAgent agent;
    WayPoint curWayPoint;
    int curWayPointID = 0;

    State curState = State.MOVE;
    float waitTimer = 0;
    float seeTimer = 0;
    LayerMask wallLayer;

    Transform enemyUI;
    Image alertFill;
    Camera cam;

    float lookAngle;
    float distanceToTarget;

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
        if(curState != State.CHASE)
          LookForPlayer();
        switch (curState)
        {
            case State.MOVE:
                MoveLogic();
                break;
            case State.WAIT_AT_CHECKPOINT:
                WaitAtWayPoint();
                break;
            case State.SEE_PLAYER:
                PlayerInView();
                break;
            case State.CHASE:
                Chase();
                break;
            default:
                break;
        }
    }

    void MoveLogic()
    {
        if (path.wayPoints.Count == 0)
        {
            Debug.LogWarning($"{name} does not have waypoints!");
            return;
        }

        curWayPoint = path.wayPoints[curWayPointID];
        Vector3 target = curWayPoint.Position;

        agent.destination = target;

        //normalize height
        target.y = transform.position.y;
        if (Vector3.Distance(transform.position, target) < minDistanceToWayPoint)
        {
            curWayPointID++;
            if (curWayPointID >= path.wayPoints.Count) curWayPointID = 0;
            curState = State.WAIT_AT_CHECKPOINT;
            waitTimer = curWayPoint.waitTime;
            lookAngle = transform.eulerAngles.y;
        }
    }

    void WaitAtWayPoint()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0) curState = State.MOVE;
        if(curWayPoint.lookAround)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            float normalizedTime = (curWayPoint.waitTime - waitTimer) / curWayPoint.waitTime;
            float shift = Mathf.Sin(2 * Mathf.PI * normalizedTime) * curWayPoint.lookAroundAngle;
            rot.y = curWayPoint.lookLeftFirst ? lookAngle - shift : lookAngle + shift;
            transform.eulerAngles = rot;
        }
    }

    void LookForPlayer()
    {
        Vector3 pos = SheepTarget.instance.Position;
        Vector3 dir = (pos - transform.position).normalized;
        distanceToTarget = Vector3.Distance(transform.position, pos);
        float angle = Vector3.Angle(transform.forward, dir);

        //Wall blocks view
        bool viewBlocked = Physics.Raycast(transform.position + Vector3.up * 0.2f, 
            pos + Vector3.up * 0.2f, viewDistance, wallLayer);

        if (curState != State.SEE_PLAYER)
        {
            if (viewBlocked) return;
            if ((distanceToTarget < viewDistance && angle < halfFOV) || distanceToTarget < awarenessDistance)
            {
                curState = State.SEE_PLAYER;
                seeTimer = SheepTarget.instance.stealthTimer;
            }
        }
        else
        {
            if (distanceToTarget > viewDistance * 1.4f || angle > halfFOV * 1.3f || viewBlocked)
            {
                seeTimer -= Time.deltaTime * 2.4f;
                if (seeTimer < 0.0f)
                {
                    curState = State.MOVE;
                    enemyUI.gameObject.SetActive(false);
                }
            }
        }
    }

    void PlayerInView()
    {
        enemyUI.gameObject.SetActive(true);
        enemyUI.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

        float stealthDuration = SheepTarget.instance.stealthTimer;

        float normalizedDistance = (viewDistance - distanceToTarget) / viewDistance;
        float increaseFactor = 0.2f + normalizedDistance * 1.4f;
        seeTimer -= Time.deltaTime * increaseFactor * awarenessLevel;


        alertFill.fillAmount = ((stealthDuration - seeTimer) / stealthDuration);
        Vector3 sheepPos = SheepTarget.instance.Position;
        Vector3 normalizedPos = new Vector3(sheepPos.x, transform.position.y, sheepPos.z);
        transform.LookAt(normalizedPos, Vector3.up);
        if(seeTimer < 0)
        {
            //Game Over
            alertFill.color = Color.red;
            curState = State.CHASE;
        } 
        else
        {
            alertFill.color = new Color(255, 154, 0);
        }
    }

    void Chase()
    {
        enemyUI.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
        agent.destination = SheepTarget.instance.Position;
        agent.speed = 5.0f;
        if (Vector3.Distance(transform.position, agent.destination) < 2.0f)
        {
            SceneManager.LoadScene(0);
        }
    }

    public enum State
    {
        MOVE, WAIT_AT_CHECKPOINT, SEE_PLAYER, CHASE
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
    }
}
