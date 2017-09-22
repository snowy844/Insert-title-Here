using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour {

   
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public float negativeJumpForce;
    public float rollForce;
    public float tapTime;
    public float slideStartAngel;
    public float slideMultiplyer;
    public float slideMultiAdditon;
    public float burm;
    public float chargetimer;
    public float chargetime;
    public int rolldistance;
    public float gravity;
    public float plusGravity;
    public float minusGravity;
    public float succRunClickMax;
    public float succRunfulClickMin;
    public float failRunClickMax;
    public float failRunClickMin;
    public float succSlideClickMax;
    public float succSlidefulClickMin;
    public float failSlideClickMax;
    public float failSlideClickMin;
    public bool sliding;
    [SerializeField]
    public float speed;
     Camera cam;

    Rigidbody rb;
    Vector3 moveDir;
    Vector3 slidingForce;
    Vector3 globalForce;
    Vector3 directionOfRoll;
    Vector3 oldPos;
    private Animator animator;
    private RaycastHit hit;

    Quaternion currentRot;
    private float movementSpeed;
    private float _doubleTapTimeA;
    private float _doubleTapTimeD;
    private float worldForwardAngle;
    private float worldRightAngle;
    

    private int jump = 0;

    private bool running;
    private bool gliding;
    private bool clicked;
   
    private bool m_grounded;
    private bool doubleTapA = false;
    private bool doubleTapD = false;
    private bool movement = true;


    void Start() {
        globalForce = Vector3.zero;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //m_grounded = true;
        gravity = .87f;
        cam = Camera.main;
       // runSpeed = runSpeed + (slideMultiplyer * .25f);
    }

    void Update() {
       
        Jump();
        Landing();
        if (Input.GetMouseButton(0) && m_grounded || Input.GetButtonDown("Slide") && m_grounded) {
            sliding = !sliding;
            running = false;
        }
        
    }

    void FixedUpdate() {
        
      
        ///Jump();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        print(m_grounded);

        //if (sliding) {
        //    if (Input.GetButton("StrafeLeft"))
        //        h = -1;
        //    else if (Input.GetButton("StrafeRight"))
        //        h = 1;
        //}
        //animator.SetFloat("_X", h);
      
        m_grounded = IsGrounded();

        checkForDoubleTap(h, v);
        checkForSlopes();
        Movement(h, v);
      //  Landing();
        //checkForSlopes();
        calculatingAngles();
        calculatingMultiplyer();

        speed = Vector3.Distance( oldPos, transform.position);
        oldPos = transform.position;
       

    }


    bool IsGrounded() {
        
        Ray ray = new Ray(transform.position, -transform.up);

        //Raycast to check if the player is grounded or not
        if (Physics.Raycast(ray, out hit, .8f)) {
         
            if (hit.collider.tag == "Map") {
                chargetimer = 0;
                //Physics.gravity = new Vector3(0, -9.87f, 0);
                return true;
            }
        }
       // sliding = false;
        return false;
    }

    void Movement(float x, float v) {
       
     
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButtonDown("Run")) {
            running = !running;
            sliding = false;
        }
       
        if (running) {
            movementSpeed = runSpeed + (slideMultiplyer * .5f); ;
            print(movementSpeed);
        }
        else {
            movementSpeed = walkSpeed + (slideMultiplyer * .25f); ;
        }
        
        if (sliding) {
            //The Player is sliding
            float xProj = animator.GetFloat("xProj");
            moveDir = transform.TransformDirection(xProj * burm, 0, v * movementSpeed);
          
            rb.MovePosition(transform.position + (moveDir * Time.deltaTime));
            rb.AddForce(moveDir * Time.deltaTime, ForceMode.VelocityChange);
        }

       if(!sliding) {

            //The Player is running
           // Vector3 _input = new Vector3(Input.GetAxis("Horizontal") * 3, 0, Input.GetAxis("Horizontal") * 3);

            //Vector3 _moveDir = cam.transform.TransformDirection(_input);
            moveDir = transform.TransformDirection(0, 0, Mathf.Clamp( v  + Mathf.Abs(x),-1,1) * movementSpeed);
            rb.MovePosition(transform.position + (moveDir * Time.deltaTime));
            //if (v < 0)
            //    rb.MovePosition(transform.position - (moveDir * Time.deltaTime));

        }





        /*
        string direction = "none";
        if (v == 0 && x == 0) direction = "none";
        if (v > 0.1) direction = "forward";
        if (v < -0.1) direction = "backward";
        if (x > 0.1) direction = "right";
        if (x < -0.1) direction = "left";
        if (v > 0.1 && x < -0.1) direction = "ForwardLeft";
        if (v > 0.1 && x > 0.1) direction = "ForwardRight";
        if (v < -0.1 && x < -0.1) direction = "BackwardLeft";
        if (v < -0.1 && x > 0.1) direction = "BackwardRight";
        switch (direction) {
            case "forward":
                rb.MovePosition(transform.position + transform.forward * movementSpeed * Time.deltaTime);
                // animator.SetBool("Running", true);
                break;

            case "backward":
                rb.MovePosition(transform.position - transform.forward * movementSpeed * Time.deltaTime);;
                //  animator.SetBool("Running", true);
                break;

            case "right":
                rb.MovePosition(transform.position + transform.right * movementSpeed * Time.deltaTime);
                directionOfRoll = transform.right;
                //animator.SetBool("Running", true);
                break;

            case "left":
                rb.MovePosition(transform.position - transform.right * movementSpeed * Time.deltaTime);
                directionOfRoll = -transform.right;
                // animator.SetBool("Running", true);
                break;

            case "ForwardLeft":
                rb.MovePosition(transform.position - (transform.right - transform.forward).normalized * movementSpeed * Time.deltaTime);
                //  animator.SetBool("Running", true);
                break;

            case "ForwardRight":
                rb.MovePosition(transform.position + (transform.right + transform.forward).normalized * movementSpeed * Time.deltaTime);
                // animator.SetBool("Running", true);
                break;

            case "BackwardLeft":
                rb.MovePosition(transform.position - (transform.right + transform.forward).normalized * movementSpeed * Time.deltaTime);
                ///animator.SetBool("Running", true);
                break;

            case "BackwardRight":
                rb.MovePosition(transform.position + (transform.right - transform.forward).normalized * movementSpeed * Time.deltaTime);
                //   animator.SetBool("Running", true);
                break;

            case "none":
                //   animator.SetBool("Running", false);
                break;
        }
        */
    }

    void checkForDoubleTap(float h, float v) {

        
        if (Input.GetKeyDown(KeyCode.A)) {
            if (Time.time < _doubleTapTimeA + tapTime) {
                doubleTapA = true;
                movement = false;
                Dodge(h, v);
            }
            _doubleTapTimeA = Time.time;

        }

        if (Input.GetKeyDown(KeyCode.D)) {
            if (Time.time < _doubleTapTimeD + tapTime) {
                doubleTapD = true;
                movement = false;
                Dodge(h, v);
            }
            _doubleTapTimeD = Time.time;

        }

    }
    
    void Jump() {
        //Player jumps up
        if (Input.GetButtonDown("Jump") && m_grounded == true) {
            rb.AddForce(transform.up * jumpForce + moveDir, ForceMode.VelocityChange);
            m_grounded = false;
            running = false;
            sliding = false;
            gravity = 0.87f;
        }
       else if (Input.GetButtonUp("Jump")) {
            jump = 1;
          
        }
        if (m_grounded == false) {
            chargetimer += Time.deltaTime;

            //Code for player to  glide.
            //Gravity gets lighter
            if (Input.GetKey(KeyCode.E) && chargetimer < chargetime || Input.GetButton("Glide") && chargetimer < chargetime) {
                gravity += plusGravity;
                //Physics.gravity = new Vector3(0, gravity, 0);
                rb.velocity = 12 * transform.forward;
                //rb.AddForce(transform.forward * 50, ForceMode.Impulse);
               
            }
            //Code that brings player back down
            //Gravity gets heavier
            if (Input.GetKeyUp(KeyCode.E) || Input.GetButtonUp("Glide") || chargetimer >= 2 ) {
                gravity -= minusGravity;
            }

           
        }
        if (m_grounded) {
            gravity = -9.87f;
        }
        Physics.gravity = new Vector3(0, gravity, 0);
        rb.AddForce(-Vector3.up * negativeJumpForce, ForceMode.VelocityChange);
    }

    void Dodge(float h, float v) {


        if (doubleTapA) {
            if (Input.GetKeyDown(KeyCode.A)) {
                if (rb.velocity.magnitude <= 0.5f && rb.velocity.magnitude >= -0.5f)
                    rb.AddForce(directionOfRoll * rollForce, ForceMode.Impulse - rolldistance);
               
            }
        }

        if (doubleTapD) {
            if (Input.GetKeyDown(KeyCode.D)) {
                if (rb.velocity.magnitude <= 0.5f && rb.velocity.magnitude >= -0.5f)
                    rb.AddForce(directionOfRoll * rollForce, ForceMode.Impulse - rolldistance);
               
            }
        }
    }

    void checkForSlopes() {

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        if (v == 0)
            v = 0.1f;
        if (h == 0)
            h = 0.1f;
        print(v);

        //Move player around based on the players mouse
        //float mouseInput = Input.GetAxis("Mouse X");
        Vector3 lookhere1 = cam.transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal") * 3, 0, Input.GetAxis("Vertical") * 3));
        Quaternion lookRot = Quaternion.LookRotation(lookhere1);
        if (lookRot.y != 0) {
            currentRot = lookRot;

        }

        //if (sliding) {
        //    float mouseInput = Input.GetAxisRaw("RightThumb");
        //    Vector3 lookhere = new Vector3(0, mouseInput * 3, 0);
        //    transform.Rotate(lookhere);
        //}


        //code for clamping area in which camera can look. Not currently used
        //float dot = Vector3.Dot(transform.forward, globalForce);
        //dot = (1 - Mathf.Clamp01(dot + .1f));
        //float yVal = Mathf.LerpAngle(transform.eulerAngles.y, Quaternion.LookRotation(globalForce).eulerAngles.y, ((Time.deltaTime * 10) * dot));

        //Vector3 down = transform.TransformDirection(Vector3.down);

        //If player is grounded slerp the player with the terrains normal
        if (m_grounded) {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, currentRot.eulerAngles.y, targetRotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (Time.deltaTime * 20) * (Mathf.Clamp(v + Mathf.Abs(h), 0f, 1)));
        }

        ////If the player isn't grounded rotate them so there transform up is the same as the worlds up
        else {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, currentRot.eulerAngles.y, targetRotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * (Mathf.Clamp(v + Mathf.Abs(h), 0, 1)) * 10);
        }
    }

    void calculatingAngles() {

        //Turns on/off the sliding mechanic
        

        //Sliding turned on will calculate global force to apply to player 
            if (sliding) {
                float forwardAngle = Vector3.Angle(transform.up, Vector3.forward);
                worldForwardAngle = forwardAngle - 90;


                float Rightangle = Vector3.Angle(transform.up, Vector3.right);
                worldRightAngle = Rightangle - 90;


                if (worldForwardAngle > slideStartAngel)
                slidingForce.z = -(worldForwardAngle - slideStartAngel) * slideMultiplyer;

                else if (worldForwardAngle < -slideStartAngel)
                slidingForce.z = Mathf.Abs((worldForwardAngle + slideStartAngel) * slideMultiplyer);

                else slidingForce.z = 0;

                if (worldRightAngle > slideStartAngel)
                slidingForce.x = -(worldRightAngle - slideStartAngel) * slideMultiplyer;
                else if (worldRightAngle < -slideStartAngel)
                slidingForce.x = Mathf.Abs((worldRightAngle + slideStartAngel) * slideMultiplyer);
                else
                slidingForce.x = 0;
            globalForce = slidingForce;
          
        }
        //the Player is running 
        if (!sliding) {
            globalForce = Vector3.zero;
        }
        rb.AddForce(globalForce, ForceMode.Force);

    }

    void Landing() {

        
            RaycastHit hitForLanding;
            Vector3 down = transform.TransformDirection(Vector3.down);

            if (!m_grounded && !clicked) {
            
                if (Physics.Raycast(transform.position, down, out hitForLanding)) {
                //Range in which the player can preform a time click to land correctly
                    if (hitForLanding.distance <= succRunClickMax && hitForLanding.distance >= succRunfulClickMin && Input.GetMouseButtonDown(0) || hitForLanding.distance <= succRunClickMax && hitForLanding.distance >= succRunfulClickMin && Input.GetButtonDown("LandRun")) {
                        jump = 2;
                        clicked = true;
                        running = true;
                    
                    //  print("landed");
                }
                    //Outside of above range is a failed click and landing
                    if (hitForLanding.distance > failRunClickMax && Input.GetMouseButtonDown(0) || hitForLanding.distance < failRunClickMin && Input.GetMouseButtonDown(0)|| hitForLanding.distance > failRunClickMax && Input.GetButtonDown("LandRun") || hitForLanding.distance < failRunClickMin && Input.GetButtonDown("LandRun")) {
                        jump = 1;
                        clicked = true;
                  //  print("didnt land");
                    }

                if (hitForLanding.distance <= succSlideClickMax && hitForLanding.distance >= succRunfulClickMin && Input.GetMouseButtonDown(0) || hitForLanding.distance <= succSlideClickMax && hitForLanding.distance >= succSlidefulClickMin && Input.GetButtonDown("LandSlide")) {
                    jump = 2;
                    clicked = true;
                    sliding = true;
                  //  print("landed");
                }
                //Outside of above range is a failed click and landing
                if (hitForLanding.distance > failSlideClickMax && Input.GetMouseButtonDown(0) || hitForLanding.distance < failSlideClickMin && Input.GetMouseButtonDown(0) || hitForLanding.distance > failSlideClickMax && Input.GetButtonDown("LandSlide") || hitForLanding.distance < failSlideClickMin && Input.GetButtonDown("LandSlide")) {
                    jump = 1;
                    clicked = true;
                    //print("didnt land");
                }
            }
            }

        }
    
    void calculatingMultiplyer() {

        if (m_grounded && clicked) {
            //failed click
            if (jump == 1) {
                jump = 0;
                slideMultiplyer = 1;
                // runSpeed = 15 + (slideMultiplyer * .25f);
                clicked = false;
            }
           // successfulClickMin clicked
            if (jump == 2) {
                jump = 0;
                slideMultiplyer += slideMultiAdditon;
               // runSpeed = 15 + (slideMultiplyer * .25f);
                clicked = false;
            }
        }
        //Didnt click
        if (m_grounded && !clicked)
            if (jump == 1) {
                jump = 0;
                slideMultiplyer = 1;
               // runSpeed = 15 + (slideMultiplyer * .25f);
            }
    }
} 

