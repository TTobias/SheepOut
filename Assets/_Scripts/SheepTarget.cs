using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SheepTarget : MonoBehaviour
{
    public float stealthTimer = 2.5f;
    public static SheepTarget instance;
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
}

