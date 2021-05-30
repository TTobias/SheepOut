using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePickup : MonoBehaviour
{

    public enum InteractivePickupTypes
    {
        Horns,
        Coconuts,
        Wool
    }
    
    [SerializeField] float interactDistance = 1.5f;
    [SerializeField] Transform textHUD;

    [SerializeField] private InteractivePickupTypes Type;

    [SerializeField] private float AddedStealthTime = 1.0F;
    
    Camera cam;
    
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector3 playerPos = SheepTarget.instance.Position;
        playerPos.y = transform.position.y;

        if (Vector3.Distance(playerPos, transform.position) < interactDistance && transform.gameObject.activeSelf)
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

    void OnInteract()
    {
        SheepTarget.instance.stealthTimer += AddedStealthTime;

        GameManager.instance.AddPickup(Type);
        
        transform.gameObject.SetActive(false);
    }
}
