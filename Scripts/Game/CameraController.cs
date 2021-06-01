using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    private bool lockCursor;
    [SerializeField]
    private Transform target;


    [SerializeField]
    private Vector3 cameraOffset = new Vector3(0f, 0f, 0f),
    aimCameraOffset = new Vector3(0f, 0f, 0f);

    [SerializeField]
    private float rotationSmoothTime = 0.1f,
    offsetSmoothTime,
    fovSmoothTime,
    mouseSensitivity,
    fov = 70,
    aimFov = 35;
    [SerializeField]
    private Vector2 pitchMinMax = new Vector2(-40, 85);

    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    Vector3 currentPosition;
    Vector3 smoothVelocityOffset;

    float currentFov;
    float smoothVelocityFov;
    float yaw;
    float pitch;
    int cameraSide = 1;
    public static Vector3 LookingAtPoint;


    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        transform.position = new Vector3(0, 0, 0);

        if (PlayerControllerV2.Aiming)
        {
            currentFov = Mathf.SmoothDamp(currentFov, aimFov, ref smoothVelocityFov, fovSmoothTime);

            transform.position = transform.TransformPoint(offset()) + target.position;
            Camera.main.fieldOfView = currentFov;
        }
        else
        {
            transform.position = transform.TransformPoint(offset()) + target.position;

            currentFov = Mathf.SmoothDamp(currentFov, fov, ref smoothVelocityFov, fovSmoothTime);
            Camera.main.fieldOfView = currentFov;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (cameraSide == 1)
            {
                cameraSide = 0;
            }
            else
            {
                cameraSide = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (cameraSide == -1)
            {
                cameraSide = 0;
            }
            else
            {
                cameraSide = -1;
            }
        }
        Vector3 offset()
        {
            if (PlayerControllerV2.Aiming)
            {
                currentPosition = Vector3.SmoothDamp(currentPosition, new Vector3(aimCameraOffset.x * cameraSide, aimCameraOffset.y, aimCameraOffset.z), ref smoothVelocityOffset, offsetSmoothTime);
            }
            else
            {
                currentPosition = Vector3.SmoothDamp(currentPosition, new Vector3(cameraOffset.x * cameraSide, cameraOffset.y, cameraOffset.z), ref smoothVelocityOffset, offsetSmoothTime);
            }
            return currentPosition;
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            LookingAtPoint = hit.point;
            Debug.DrawLine(transform.position, LookingAtPoint, Color.red);
        }
    }
}