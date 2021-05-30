using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePickup : MonoBehaviour
{
    
    [SerializeField] float interactDistance = 1.5f;
    [SerializeField] Transform textHUD;
    [SerializeField] Transform lever;

    [SerializeField] private float AddedStealthTime = 1.0F;
    
    Camera cam;
    
    private void Start()
    {
        textHUD.gameObject.SetActive(false);
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 playerPos = SheepTarget.instance.Position;
        playerPos.y = transform.position.y;

        if (Vector3.Distance(playerPos, transform.position) < interactDistance)
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

    public void OnInteract()
    {
        
        
        
        
    }
}
