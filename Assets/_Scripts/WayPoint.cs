using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPoint : MonoBehaviour
{
    public Vector3 Position { get { return transform.position; } }

    [Header("Wait Time")]
    [Tooltip("How long does the sheep wait after reaching a checkpoint")]
    public float waitTime = 2.0f;

    [Header("Look Around Values")]
    [Tooltip("Sheep looks around when reaching waypoint")]
    public bool lookAround = false;
    public float lookAroundAngle = 30;
    public bool lookLeftFirst = false;

    LayerMask floorLayer = (1 << 0) | (1 << 6);
    void OnDrawGizmos()
    {
        Gizmos.color = lookAround ? Color.red : Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    private void Update()
    {
        //Snap to ground
        if (Application.isEditor)
        {      
            if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, 
                out RaycastHit hit, 200.0f, floorLayer))
            {
                Debug.Log(hit.transform.gameObject.layer);
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }

        }
    }
}

