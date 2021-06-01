using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerController : NetworkBehaviour
{
    Vector2 Direction;
    Vector2 RawMovementInput;
    Vector2 SmoothMovementInput;

    Vector2 SmoothMovementVelocity;
    float speedVelocity;

    public float MovementSmoothing;
    public GameObject CameraMountPoint;
    public GameObject Head;
    public float WalkSpeed = 5;
    public float RunSpeed = 10;
    public float SpeedSmooth = 0.2f;
    public float CameraAdjustmentDivisor = 360;
    public Vector3 Gravity = new Vector3(0, -9.82f, 0);
    public float hardLandSpeed = 4;
    public float jumpHeight = 1.5f;

    Vector3 Velocity;
    float speed;
    bool running;
    bool FirstFrame = true;

    Transform spine;
    Transform chest;
    Transform armature;

    CharacterController controller;
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            Velocity += Gravity * Time.deltaTime;
            Debug.Log("Gravity: " + Velocity);

            running = Input.GetKey(KeyCode.LeftControl);
            speed = Mathf.SmoothDamp(speed, running ? RunSpeed : WalkSpeed, ref speedVelocity, SpeedSmooth);

            Direction += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
            Direction = new Vector2(Mathf.Clamp(Direction.x, -89, 89), Direction.y);
            transform.eulerAngles = Direction * new Vector2(0, 1);

            RawMovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            RawMovementInput = RawMovementInput.normalized;

            SmoothMovementInput = new Vector2(
                Mathf.SmoothDamp(SmoothMovementInput.x, RawMovementInput.x, ref SmoothMovementVelocity.x, MovementSmoothing),
                Mathf.SmoothDamp(SmoothMovementInput.y, RawMovementInput.y, ref SmoothMovementVelocity.y, MovementSmoothing)
            );

            Debug.Log("Raw: " + RawMovementInput);
            Debug.Log("Smooth: " + SmoothMovementInput);
            Debug.Log("Grounded: " + controller.isGrounded);


            controller.Move((transform.forward * SmoothMovementInput.y + transform.right * SmoothMovementInput.x) * speed * Time.deltaTime + Velocity * Time.deltaTime);

            Debug.Log("speed: " + speed);
            Debug.Log("Magnitude: " + SmoothMovementInput.magnitude * speed);



            if (controller.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    float jumpVelocity = Mathf.Sqrt(2 * Gravity.magnitude * jumpHeight);
                    Velocity = new Vector3(0, jumpVelocity, 0);
                    anim.SetTrigger("Jump");
                }
                else
                {
                    if (Velocity.magnitude > hardLandSpeed)
                    {
                        anim.SetTrigger("HardLand");
                        Debug.Log("hardLand");
                    }

                    Velocity = new Vector3(0, -Gravity.y * Time.deltaTime, 0);
                    anim.SetBool("IsGrounded", true);
                }
            }
            else
            {
                anim.SetBool("IsGrounded", false);
            }

            UpdateAnimations();
        }
    }
    void UpdateAnimations()
    {
        anim.SetFloat("WalkState", SmoothMovementInput.magnitude * speed / 6);
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

    void LateUpdate()
    {
        if (IsLocalPlayer)
        {
            Vector3 chestRotation = chest.eulerAngles;
            Vector3 spineRotation = spine.eulerAngles;

            chestRotation.x = Direction.x;
            chestRotation.y = Direction.y;
            spineRotation.x = Direction.x / 2;

            spineRotation.y = spineRotation.y - (spineRotation.y - Direction.y);

            if (spineRotation.x > 50)
            {
                spineRotation.x -= 180;
            }
            if (Mathf.Abs(spineRotation.y - Direction.y) > 90)
            {
                spineRotation.y += 180;
            }
            spine.eulerAngles = spineRotation;
            chest.eulerAngles = chestRotation;
            armature.localPosition = new Vector3(0, 0.9819709f, -Direction.x / CameraAdjustmentDivisor);
        }
    }
}
