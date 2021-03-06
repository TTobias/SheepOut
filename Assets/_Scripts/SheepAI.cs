using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class SheepAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minDistanceToWayPoint = 0.2f;
    [SerializeField] float viewDistance = 20;
    [Tooltip("influences how fast sheep sees player")]
    [SerializeField] float awarenessLevel = 1.0f;
    [Tooltip("If player moves too close to enemy fov ignored!")]
    [SerializeField] float awarenessDistance = 0.5f;
    [SerializeField] float halfFOV = 60;
    [SerializeField] int startWayPoint = 0;
    AudioSource bahSource;

    [Header("WayPoints")]
    [SerializeField] WayPointPath path;

    [Header("Referenes")]
    [SerializeField] Transform weaponIK;
    [SerializeField] AudioClip[] bahSounds;

    NavMeshAgent agent;
    WayPoint curWayPoint;
    int curWayPointID = 0;

    State curState = State.MOVE;
    float waitTimer = 0;
    float seeTimer = 0;
    float lookAroundTimer = 0;
    LayerMask wallLayer;

    Transform enemyUI;
    Image alertFill;
    Camera cam;
    Animator anim;

    float lookAngle;
    float distanceToTarget;
    bool stillInview;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        bahSource = GetComponentInChildren<AudioSource>();
        wallLayer = (1 << 7) | (1 << 9);
        enemyUI = transform.Find("EnemyUI");
        alertFill = enemyUI.Find("Fill").GetComponent<Image>();
        enemyUI.gameObject.SetActive(false);
        cam = Camera.main;
        if (startWayPoint >= path.wayPoints.Count)
        {
            curWayPointID = 0;
            Debug.LogWarning("Start waypoint out of bounds");
        }
        curWayPointID = startWayPoint;

        //Random Weapon
        int rng = Random.Range(0, weaponIK.childCount);
        Transform weapon = weaponIK.GetChild(rng);
        weapon.gameObject.SetActive(true);
    }

    void Update()
    {
        if(curState != State.CHASE)
          LookForPlayer();
        switch (curState)
        {
            case State.MOVE:
                MoveLogic();
                anim.SetBool("move", agent.velocity.magnitude > 0.5f);
                break;
            case State.WAIT_AT_CHECKPOINT:
                WaitAtWayPoint();
                anim.SetBool("move", false);
                break;
            case State.LOOK_AROUND:
                LookAround();
                anim.SetBool("move", false);
                break;
            case State.SEE_PLAYER:
                PlayerInView();
                anim.SetBool("move", false);
                break;
            case State.CHASE:
                Chase();
                anim.SetBool("move", true);
                break;
            default:
                break;
        }

        //Update anim
        anim.SetInteger("nextIdle", Random.Range(0, 2));
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

        agent.isStopped = false;
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

        if (curWayPoint.rotateToWayPointDir)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.Euler(transform.rotation.x, curWayPoint.transform.eulerAngles.y, transform.rotation.z),
                Time.deltaTime * (180 / curWayPoint.waitTime));
        }

        if (waitTimer <= 0)
        {
            if (curWayPoint.lookAround)
            {
                curState = State.LOOK_AROUND;
                lookAngle = transform.eulerAngles.y;
                lookAroundTimer = curWayPoint.lookTime;
            }
            else
                curState = State.MOVE;
        }
    }

    void LookAround()
    {
        lookAroundTimer -= Time.deltaTime;
        if (lookAroundTimer < 0) curState = State.MOVE;
        Vector3 rot = transform.rotation.eulerAngles;
        float normalizedTime = (curWayPoint.lookTime - lookAroundTimer) / curWayPoint.lookTime;
        float shift = Mathf.Sin(2 * Mathf.PI * normalizedTime) * curWayPoint.lookAroundAngle;
        rot.y = curWayPoint.lookLeftFirst ? lookAngle - shift : lookAngle + shift;
        transform.eulerAngles = rot;
    }

    void LookForPlayer()
    {
        Vector3 pos = SheepTarget.instance.Position;
        Vector3 dir = (pos - transform.position).normalized;
        distanceToTarget = Vector3.Distance(transform.position, pos);
        float angle = Vector3.Angle(transform.forward, dir);

        //Wall blocks view
        bool viewBlocked = Physics.Raycast(transform.position, dir, distanceToTarget, wallLayer);

        if (curState != State.SEE_PLAYER)
        {
            if (viewBlocked) return;
            if ((distanceToTarget < viewDistance && angle < halfFOV) || distanceToTarget < awarenessDistance)
            {
                curState = State.SEE_PLAYER;
                seeTimer = SheepTarget.instance.stealthTimer;
                BahSound();
            }
        }
        else
        {
            if (distanceToTarget > viewDistance * 1.2f || angle > halfFOV * 1.1f || viewBlocked)
            {
                stillInview = false;
                seeTimer += Time.deltaTime * 3.2f;
                if (seeTimer > SheepTarget.instance.stealthTimer)
                {
                    curState = State.MOVE;
                    enemyUI.gameObject.SetActive(false);
                }
            }
            else
            {
                stillInview = true;
            }
        }
    }

    void PlayerInView()
    {
        agent.isStopped = true;
        enemyUI.gameObject.SetActive(true);
        enemyUI.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

        float stealthDuration = SheepTarget.instance.stealthTimer;

        float normalizedDistance = (viewDistance - distanceToTarget) / viewDistance;
        float increaseFactor = 0.3f + normalizedDistance * 0.7f;
        float runFactor = WolfController.Running ? 1.8f : 1.0f;

        float angle = Vector3.Angle(transform.forward, cam.transform.forward);
        float outsideOfCamViewModifier = angle < 90 ? 0.4f : 1.0f;

        if (stillInview)
            seeTimer -= Time.deltaTime * increaseFactor * 
                awarenessLevel * runFactor * outsideOfCamViewModifier;


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
            if (!stillInview) alertFill.color *= 0.6f;
        }
    }

    void Chase()
    {
        agent.isStopped = false;
        enemyUI.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
        agent.destination = SheepTarget.instance.Position;
        agent.speed = 5.5f;
        if (WolfController.Running) agent.speed = 7.5f;
        if (Vector3.Distance(transform.position, agent.destination) < 2.0f)
        {
            GameManager.GameOver();
        }
    }

    void BahSound()
    {
        AudioClip a = bahSounds[Random.Range(0, bahSounds.Length)];
        bahSource.clip = a;
        bahSource.pitch = Random.Range(0.8f, 1.2f);
        bahSource.Play();
    }

    public enum State
    {
        MOVE, WAIT_AT_CHECKPOINT, SEE_PLAYER, CHASE, LOOK_AROUND
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
