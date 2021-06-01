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
        //kind of in the same position that the HitDirection script is, somewhat works but has a few extremely weird issues that i really dont understand. the spawned particle is in the wrong direction, but not in a logical way.
        GameObject gun = this.transform.parent.transform.parent.parent.transform.gameObject;
        hitPoint = gun.GetComponent<GunScript>().hit;

        Roundtrace.transform.rotation = Quaternion.LookRotation(hitPoint.point - transform.position);
        Debug.Log(Roundtrace.transform.eulerAngles);
        Roundtrace.Play();
        Smoke.Play();
    }
}
