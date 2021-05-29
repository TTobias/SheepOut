using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] Gate gate;
    [SerializeField] float interactDistance = 1.5f;
    [SerializeField] Transform textHUD;

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

            if (Input.GetKeyDown(KeyCode.E))
                OnInteract();
        }
        else
        {
            textHUD.gameObject.SetActive(false);
        }
    }

    public void OnInteract()
    {
        switched = true;
        gate?.Open();
    }
}
