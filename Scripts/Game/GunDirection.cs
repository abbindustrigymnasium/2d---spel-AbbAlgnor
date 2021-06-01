using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class GunDirection : NetworkBehaviour
{
    public float RotateSmoothTime = 0.1f;
    private float AngularVelocity = 0.0f;
    public Vector3 rotationOffset;

    public Transform hand;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            var target_rot = Quaternion.LookRotation(LookAtPoint.LookingAtPoint - transform.position);
            var delta = Quaternion.Angle(transform.rotation, target_rot);
            if (delta > 0.0f)   //smoohly makes the weapon look at the hitpoint
            {
                var t = Mathf.SmoothDampAngle(delta, 0.0f, ref AngularVelocity, RotateSmoothTime);
                t = 1.0f - t / delta;
                transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, t);
            }

                Debug.DrawLine(transform.position, LookAtPoint.LookingAtPoint, Color.green);
        }
    }
}
