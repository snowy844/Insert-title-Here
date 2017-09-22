using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCamera : MonoBehaviour {

    [SerializeField]
    private Transform target = null;
    [SerializeField]
    private float distance = 3.0f;
    [SerializeField]
    private float height = 1.0f;
    [SerializeField]
    private float damping = 5.0f;
    [SerializeField]
    private bool smoothRotation = true;
    [SerializeField]
    private float rotationDamping = 10.0f;

    [SerializeField]
    private Vector3 targetLookAtOffset; // allows offsetting of camera lookAt, very useful for low bumper heights
    
    [SerializeField]
    private float bumperDistanceCheck = 2.5f; // length of bumper ray
    [SerializeField]
    private float bumperCameraHeight = 1.0f; // adjust camera height while bumping
    [SerializeField]
    private Vector3 bumperRayOffset; // allows offset of the bumper ray from target origin
    PlayerMovement speed;
    Vector3 offset = new Vector3(0,4,0);
    public Transform player;
    Vector3 lerpPos;
    /// <Summary>
    /// If the target moves, the camera should child the target to allow for smoother movement. DR
    /// </Summary>
    private void Awake() { 
        speed = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        
    }

    private void FixedUpdate() {
        Vector3 wantedPosition = target.TransformPoint(0, height, -distance);
        
       // check to see if there is anything behind the target
       RaycastHit hit;
        Vector3 back = target.transform.TransformDirection(-1 * Vector3.forward);
        float rightH = Input.GetAxis("RightThumb");
        float rightV = Input.GetAxis("RightThumbV");
      
        Vector3 lookhere = new Vector3(/*(3 * Time.deltaTime) **/ rightV * 3, /*(3*Time.deltaTime)**/rightH * 3, 0);
        transform.eulerAngles += lookhere;
        
        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(target.TransformPoint(bumperRayOffset), back, out hit, bumperDistanceCheck)
            && hit.transform != target) // ignore ray-casts that hit the user. DR
        {

            // clamp wanted position to hit position
            wantedPosition.x = hit.point.x;
            wantedPosition.z = hit.point.z;
            wantedPosition.y = Mathf.Lerp(hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * damping);
        }
        float rotVal = Mathf.Clamp( speed.speed,0 , 10);
        if (rotVal > 1)
            rotVal = 0;
        Quaternion playerRot = player.rotation;
        
        playerRot.x = playerRot.x - .02f;
        //playerRot.z = 0;
        if (!speed.sliding) {

            transform.rotation = Quaternion.Slerp(transform.rotation, playerRot, (Time.deltaTime * rotationDamping) * rotVal);
        }

        else {
            transform.rotation = /*transform.rotation;*/Quaternion.Slerp(transform.rotation, target.rotation, (Time.deltaTime * rotationDamping) * rotVal);
        }
        //  transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);

        //Vector3 lookPosition = target.TransformPoint(wantedPosition);

        // // if (new Vector2(rightH, rightV).magnitude > 0) {

        // Quaternion wantedRotation = Quaternion.LookRotation (lookPosition- target.position, target.up);

        //     transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, (Time.deltaTime * rotationDamping) * speed.speed);

        // }

    }
}
