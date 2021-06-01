using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class TestMovement : NetworkBehaviour
{
    Vector2 Direction;
    Vector3 Movement;
    public Camera camera;
    public float speed = 5;
    public GameObject CameraMountPoint;
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            Direction += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
            transform.eulerAngles = Direction;

            Movement = new Vector3(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0f, Input.GetAxisRaw("Vertical") * speed * Time.deltaTime);
            transform.Translate(Movement);
        }
    }
}
