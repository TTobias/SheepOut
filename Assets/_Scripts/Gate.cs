using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] float movementFactor = 3.5f;
    public void Open()
    {
        StartCoroutine(OpenRoutine(transform.position, 
            transform.position + transform.up * movementFactor, 1.2f));
    }

    IEnumerator OpenRoutine(Vector3 start, Vector3 goal, float speed)
    {
        float step = (speed / (start - goal).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step;
            transform.position = Vector3.Lerp(start, goal, t);
            yield return new WaitForFixedUpdate();   
        }
        transform.position = goal;
    }
}
