using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPoint : MonoBehaviour
{
    public Vector3 Position { get { return transform.position; } }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    private void Update()
    {
        //Snap to ground
        if (Application.isEditor)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200.0f,
                LayerMask.NameToLayer("Ground")))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }

        }
    }
}

