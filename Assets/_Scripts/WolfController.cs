using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{

    public CharacterController controller;
    public Camera cam;
    public Transform player;

    public float speed = 3.5f;
    public float runSpeed = 6.0f;
    public float gravity = -9.81f;

    public Vector3 velocity;
    
    public float sensitivity = 100f;

    public float xRotation = 0f;
    public static bool Running = false;

    private Vector3 InputRotation;

    private void Start() {
        cam = GetComponentInChildren<Camera>();
        controller = this.GetComponent<CharacterController>();
        player = this.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void Update() {
        
        //Copied from other project
        /*
        ///Interact etc (Check for Vehicle)
        if (Input.GetButtonDown("Interact")) {
            RaycastHit hit;
            Physics.Raycast(cam.transform.position, cam.transform.forward, out hit);

            Debug.Log("created Vehicle Raycast");

            if (hit.collider != null) {
                MovementBehavior tmpVehicle = hit.collider.GetComponent<MovementBehavior>();
                if (tmpVehicle != null) {
                    inputManager.setMovementBehavior(tmpVehicle);
                }
                else {
                    Debug.Log("No Vehicle found");
                }
            }
            /*
            if (hit.collider != null) {
                VehicleMount tmpVehicle = hit.collider.GetComponent<VehicleMount>();
                if (tmpVehicle != null) {
                    inputManager.setMovementBehavior(tmpVehicle.vehicleMovement);
                }
                else {
                    Debug.Log("No Vehicle found");
                }
            }
        }*/
        ///Camera
        
    }

    public void FixedUpdate(){
        
        
        ///movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        ///gravity
        move += Vector3.down*gravity;

        //apply movement
        Running = Input.GetButton("Run");
        float s = Running ? runSpeed : speed;
        controller.Move(move * s * Time.fixedDeltaTime);
    
       
    }

    public void LateUpdate()
    {
        if (Time.timeScale == 0)
            return;

        InputRotation.x = Input.GetAxis("Mouse X") * sensitivity;
        InputRotation.y = Input.GetAxis("Mouse Y") * sensitivity;
        
        xRotation -= InputRotation.y * Time.fixedDeltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        player.Rotate(Vector3.up * InputRotation.x * Time.fixedDeltaTime);
    }
    
}
