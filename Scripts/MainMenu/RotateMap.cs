using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMap : MonoBehaviour
{
    [SerializeField]
    private float Speed = 10; //degrees per second 

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0f, Speed * Time.deltaTime, 0f); //slowly rotates the map in the main menu...
    }
}
