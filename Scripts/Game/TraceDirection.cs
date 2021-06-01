using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceDirection : MonoBehaviour
{
    public ParticleSystem Roundtrace;
    public ParticleSystem Smoke;
        RaycastHit hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gun = this.transform.parent.transform.parent.parent.transform.gameObject;
        hitPoint = gun.GetComponent<GunScript>().hit;

        //Roundtrace.transform.rotation = Quaternion.LookRotation(hitPoint.point - transform.position);
        Debug.Log(Roundtrace.transform.eulerAngles);
        Roundtrace.Play();
        Smoke.Play();
    }
}
