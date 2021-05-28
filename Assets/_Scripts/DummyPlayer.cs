using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DummyPlayer : MonoBehaviour
{
    public float stealthTimer = 0.8f;
    public static DummyPlayer instance;
    public Vector3 Position { get { return transform.position; } }

    private void Awake()
    {
        instance = this;    
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    private void Update()
    {
        //Snap to ground
        if (Application.isEditor)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, 
                out RaycastHit hit, 200.0f, LayerMask.NameToLayer("Ground")))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }

        }
    }
}

