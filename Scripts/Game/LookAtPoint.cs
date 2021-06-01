using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPoint : MonoBehaviour
{
    public static Vector3 LookingAtPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit; //sets the public point that the camera is looking at that all the other script uses, 
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            LookingAtPoint = hit.point;
            Debug.DrawLine(transform.position, LookingAtPoint, Color.red);
        }
    }
}
