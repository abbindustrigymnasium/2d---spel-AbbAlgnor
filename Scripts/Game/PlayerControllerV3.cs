using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

//very similar to v2, just generally cleaned up
public class PlayerControllerV3 : NetworkBehaviour
{
    //all of the settings visible in the inspector

    //camera things
    [SerializeField]
    private GameObject Head,    //used for toggling visibility for client
    CameraMountPoint;

    [SerializeField]
    [Header("Movement")]
    private float walkSpeed = 2;
    [SerializeField]
    private float runSpeed = 6,
     walkStrafeSpeed = 1.5f,
     runStafeSpeed = 4f,
     jumpHeight = 2;
    [SerializeField]
    [Range(0, 1)]
    private float airControllPercent = 0.2f,
    gravity = -9.82f,
    CameraAdjustmentDivisor = 200f;


    [Header("Smoothing")]
    [SerializeField]
    private float speedSmoothTime = 0.1f;


    [SerializeField]
    [Header("Animation")]
    private float hardLandSpeed;


    //global varables to be used in other scripts
    public static bool Aiming;


    float speedSmoothVelocityX;
    float speedSmoothVelocityZ;
    float currentSpeedX;
    float currentSpeedZ;
    float velocityY;
    float currentSpeed;
    float AngularVelocity;
    Vector3 movementSpeed;

    bool running;
    bool lastGroundState;
    bool FirstFrame = true;

    Vector2 input;
    Vector2 inputDir;

    Vector2 cameraInput;


    Animator anim;
    Transform spine;
    Transform chest;
    Transform head;
    Transform armature;
    CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer) //just simply checks if the script is run by a local player, otherwise turn of character controller and do nothing
        {
            controller = GetComponent<CharacterController>();
            controller.enabled = false;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;

        Transform cameraTransform = Camera.main.gameObject.transform;  //Find main camera which is part of the scene instead of the prefab
        cameraTransform.parent = CameraMountPoint.transform;  //Make the camera a child of the mount point
        cameraTransform.position = CameraMountPoint.transform.position;  //Set position/rotation same as the mount point
        cameraTransform.rotation = CameraMountPoint.transform.rotation;
        Head.SetActive(false);
        Camera.main.fieldOfView = PlayerPrefs.GetFloat("FOV");

        spine = transform.Find("Armature/Root/Hip/Spine");
        chest = transform.Find("Armature/Root/Hip/Spine/Chest");
        armature = transform.Find("Armature");
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        Head.SetActive(false);
        Camera.main.fieldOfView = PlayerPrefs.GetFloat("FOV");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer)//checks if is run by a local player
        {
            return;
        }
        movement(); //all the movement stuff
        inputs();   //all the inputs, would ideally be run before movement, but that causes a few issues i cant bother fixing.
        animations();   //sets the animations
    }

    void FixedUpdate()
    {
        if (FirstFrame) //this is a supid and retarted way of doing it, but it is the only way i found that actually works. it sets the players spawnpoing, host and client starts on different sides of the map.
        {
            bool mode = PlayerPrefs.GetInt("Mode") == 0 ? false : true;
            if (mode)
            {
                transform.position = new Vector3(12, 1, 20);
                Debug.LogWarning("Host at " + transform.position);
            }
            else
            {
                transform.position = new Vector3(-14, 1, 8);
                Debug.LogWarning("Client at " + transform.position);
            }
        }
        FirstFrame = false;
    }

    void inputs() // gets the inputs, mouse inputs is directly accesed in the movement part.
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;

        Aiming = Input.GetMouseButton(1);

        running = Input.GetKey(KeyCode.LeftControl) & !Aiming;
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
            Debug.Log(controller.isGrounded);
            jump();
        }
    }



    void movement()
    {
        rotation();     //gets and sets the players rotation
        translation();  //sets the players movement.
    }
    void rotation()  //gets and sets the players rotation
    {
        cameraInput += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        cameraInput = new Vector2(Mathf.Clamp(cameraInput.x, -89, 89), cameraInput.y);
        transform.eulerAngles = cameraInput * new Vector2(0, 1);
    }
    void translation()  //sets the players movement.
    {
        velocityY += gravity * Time.deltaTime;  //adds gravity to the players y velocity

        Vector2 targetSpeed = new Vector2(input.x * (running ? runStafeSpeed : walkStrafeSpeed), input.y * (running ? runSpeed : walkSpeed));   //sets the players target speed, runspeed/walkspeed/strafespeed are all configurable in the inspector
        Vector2 targetSpeedDir = targetSpeed.normalized;

        currentSpeedX = Mathf.SmoothDamp(currentSpeedX, targetSpeed.x * Mathf.Abs(targetSpeedDir.x), ref speedSmoothVelocityX, SpeedState(speedSmoothTime));    //smoothly changes the players movement, without it the player just feels bad
        currentSpeedZ = Mathf.SmoothDamp(currentSpeedZ, targetSpeed.y * Mathf.Abs(targetSpeedDir.y), ref speedSmoothVelocityZ, SpeedState(speedSmoothTime));    //smoothly changes the players movement, without it the player just feels bad



        Vector3 velocity = transform.forward * currentSpeedZ + Vector3.up * velocityY + transform.right * currentSpeedX;    //stores all the current velocities in a single vector3

        controller.Move(velocity * Time.deltaTime); //moves the player

        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)  //used for setting animations
        {
            if (velocityY < hardLandSpeed)
            {
                anim.SetTrigger("HardLand");
                Debug.Log("HardLand");
            }
            velocityY = -1f;
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            anim.SetBool("IsGrounded", false);
        }
    }

    float SpeedState(float value)   //gets wheither the player is grounded or not, controlls the players controll percentage
    {
        if (controller.isGrounded)
        {
            return value;
        }
        else
        {
            return value / airControllPercent;
        }
    }
    void jump() //just a simple gunscript, pretty self explanatory
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
            anim.SetTrigger("Jump");
        }
    }

    void LateUpdate()   //updates the rig, used for the spine when looking down etc, needs to be run in LateUpdate() due to how animations in unity works.
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        Vector3 chestRotation = chest.eulerAngles;
        Vector3 spineRotation = spine.eulerAngles;
        Vector3 rotation = new Vector3(cameraInput.x, cameraInput.y, 0);
        //Vector3 headRotation = new Vector3(CameraT.eulerAngles.x, CameraT.eulerAngles.y, head.eulerAngles.z);

        chestRotation.x = rotation.x;
        chestRotation.y = rotation.y;
        spineRotation.x = rotation.x / 2;

        spineRotation.y = spineRotation.y - (spineRotation.y - rotation.y);

        if (spineRotation.x > 50)   //needs to be done to avoid a weird issue with euler angles, ideally i would use quaternions, but i dont really understand them.
        {
            spineRotation.x -= 180;
        }
        if (Mathf.Abs(spineRotation.y - rotation.y) > 90)   //needs to be done to avoid a weird issue with euler angles, ideally i would use quaternions, but i dont really understand them.
        {
            spineRotation.y += 180;
        }
        spine.eulerAngles = spineRotation;  //applies the angle offset to the bones.
        chest.eulerAngles = chestRotation;  //applies the angle offset to the bones.
        //chest.LookAt(ThirdPersonCamera.LookingAtPoint);
        //head.eulerAngles = headRotation;
        //root.eulerAngles = new Vector3 (-90f, cameraAngleOffset(), 0f) + transform.eulerAngles;
        armature.localPosition = new Vector3(0, 0.9819709f, -cameraInput.x / CameraAdjustmentDivisor);  //offsets the players body wihtin the character controller to avoid clipping in walls when looking forward/down/up
    }
    void animations()   //set the walkstate
    {
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f);
        anim.SetFloat("WalkState", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }
}
