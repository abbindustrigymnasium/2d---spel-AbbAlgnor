using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerControllerV2 : NetworkBehaviour
{
    [SerializeField]
    public GameObject CameraMountPoint;
    public GameObject Head;

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
    gravity = -9.82f;


    [SerializeField]
    [Header("Smoothing")]
    private float turnSmoothTime = 0.1f;
    [SerializeField]
    private float speedSmoothTime,
    RotateSmoothTime = 0.1f;


    [SerializeField]
    [Header("Animation")]
    private float hardLandSpeed;


    //global varables to be used in other scripts
    public static bool Aiming;


    float turnSmoothVelocity;
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


    Animator anim;
    Transform CameraT;
    Transform spine;
    Transform chest;
    Transform head;
    Transform root;
    CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
        CameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        spine = transform.Find("Armature/Root/Hip/Spine");
        chest = transform.Find("Armature/Root/Hip/Spine/Chest");
        head = transform.Find("Armature/Root/Hip/Spine/Chest/Neck/Head");
        root = transform.Find("Armature/Root");

        if (IsLocalPlayer)
        {

            Head.SetActive(false);
            Camera.main.fieldOfView = PlayerPrefs.GetFloat("FOV");
        }
    }

    // Update is called once per frame
    void Update()
    {
        inputs();
        movement();
        animations();
       //cameraAngleOffset();
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

    void movement()
    {
        rotation();
        translation();
    }
    void rotation()
    {
        //camera rotation
        float PlayerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, CameraT.eulerAngles.y, ref turnSmoothVelocity, SpeedState(turnSmoothTime));

        transform.eulerAngles = new Vector3(0, PlayerRotation, 0);
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
                anim.SetTrigger("hardLand");
                Debug.Log("hardLand");
            }
            velocityY = -1f;
            anim.SetBool("sGrounded", true);
        }
        else
        {
            anim.SetBool("isGrounded", false);
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

    void inputs()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;

        Aiming = Input.GetMouseButton(1);

        running = Input.GetKey(KeyCode.LeftControl) & !Aiming;
        if (Input.GetButtonDown("Jump"))
        {
            jump();
        }
    }
    void animations()
    {
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f);
        anim.SetFloat("walkState", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
            anim.SetTrigger("isJumping");
        }
    }
    void LateUpdate()
    {
        Vector3 chestRotation = chest.eulerAngles;
        Vector3 spineRotation = spine.eulerAngles;
        Vector3 cameraRotation = CameraT.eulerAngles;
        //Vector3 headRotation = new Vector3(CameraT.eulerAngles.x, CameraT.eulerAngles.y, head.eulerAngles.z);

        chestRotation.x = cameraRotation.x;
        chestRotation.y = cameraRotation.y;
        spineRotation.x = cameraRotation.x / 2;

        spineRotation.y = spineRotation.y - (spineRotation.y - cameraRotation.y);

        if (spineRotation.x > 50)
        {
            spineRotation.x -= 180;
        }
        if (Mathf.Abs(spineRotation.y - cameraRotation.y) > 90)
        {
            spineRotation.y += 180;
        }
        spine.eulerAngles = spineRotation;
        chest.eulerAngles = chestRotation;
        //chest.LookAt(ThirdPersonCamera.LookingAtPoint);
        //head.eulerAngles = headRotation;
        //root.eulerAngles = new Vector3 (-90f, cameraAngleOffset(), 0f) + transform.eulerAngles;
    }

    // float cameraAngleOffset()
    // {
    //     Vector3 localOffset = transform.InverseTransformPoint(CameraController.LookingAtPoint);
    //     float angleOffset = Mathf.Tan(localOffset.x / localOffset.z) * Mathf.Rad2Deg;

    //     return angleOffset;
    // }
}
