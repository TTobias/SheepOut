using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] Gate gate;
    [SerializeField] float interactDistance = 1.5f;
    [SerializeField] Transform textHUD;
    [SerializeField] Transform lever;

    Camera cam;
    bool switched;
    private void Start()
    {
        textHUD.gameObject.SetActive(false);
        cam = Camera.main;
        switched = false;
    }

    private void Update()
    {
        Vector3 playerPos = SheepTarget.instance.Position;
        playerPos.y = transform.position.y;

        if (Vector3.Distance(playerPos, transform.position) < interactDistance && !switched)
        {
            textHUD.gameObject.SetActive(true);
            textHUD.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

            if (Input.GetMouseButtonDown(0))
                OnInteract();
        }
        else
        {
            textHUD.gameObject.SetActive(false);
        }
    }

    IEnumerator MoveSwitch()
    {
        float z = 0;
        while(z > -40)
        {
            z -= Time.deltaTime * 42.0f;
            lever.localEulerAngles = new Vector3(0, 0, z);
            yield return new WaitForFixedUpdate();
        }
    }

    public void OnInteract()
    {
        StartCoroutine(MoveSwitch());
        switched = true;
        gate?.Open();
    }
}
