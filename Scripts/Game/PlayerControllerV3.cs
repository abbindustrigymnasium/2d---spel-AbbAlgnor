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
        if (!IsLocalPlayer)
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
        if (!IsLocalPlayer)
        {
            return;
        }
        movement();
        inputs();
        animations();
    }

    void FixedUpdate()
    {
        if (FirstFrame)
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

    void inputs()
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
        translation();
    }
    void rotation()
    {
        cameraInput += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        cameraInput = new Vector2(Mathf.Clamp(cameraInput.x, -89, 89), cameraInput.y);
        transform.eulerAngles = cameraInput * new Vector2(0, 1);
    }
    void translation()
    {
        velocityY += gravity * Time.deltaTime;

        Vector2 targetSpeed = new Vector2(input.x * (running ? runStafeSpeed : walkStrafeSpeed), input.y * (running ? runSpeed : walkSpeed));
        Vector2 targetSpeedDir = targetSpeed.normalized;

        currentSpeedX = Mathf.SmoothDamp(currentSpeedX, targetSpeed.x * Mathf.Abs(targetSpeedDir.x), ref speedSmoothVelocityX, SpeedState(speedSmoothTime));
        currentSpeedZ = Mathf.SmoothDamp(currentSpeedZ, targetSpeed.y * Mathf.Abs(targetSpeedDir.y), ref speedSmoothVelocityZ, SpeedState(speedSmoothTime));



        Vector3 velocity = transform.forward * currentSpeedZ + Vector3.up * velocityY + transform.right * currentSpeedX;

        controller.Move(velocity * Time.deltaTime);

        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
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

    float SpeedState(float value)
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
    void jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
            anim.SetTrigger("Jump");
        }
    }

    void LateUpdate()
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

        if (spineRotation.x > 50)
        {
            spineRotation.x -= 180;
        }
        if (Mathf.Abs(spineRotation.y - rotation.y) > 90)
        {
            spineRotation.y += 180;
        }
        spine.eulerAngles = spineRotation;
        chest.eulerAngles = chestRotation;
        //chest.LookAt(ThirdPersonCamera.LookingAtPoint);
        //head.eulerAngles = headRotation;
        //root.eulerAngles = new Vector3 (-90f, cameraAngleOffset(), 0f) + transform.eulerAngles;
        armature.localPosition = new Vector3(0, 0.9819709f, -cameraInput.x / CameraAdjustmentDivisor);
    }
    void animations()
    {
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f);
        anim.SetFloat("WalkState", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }
}
