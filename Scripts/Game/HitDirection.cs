using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDirection : MonoBehaviour
{
    public ParticleSystem Hit;
    RaycastHit hitPoint;

    // Start is called before the first frame update
    void Start()
    {
        //this for some reason only partly works, the x and z position is right, and the rotation is right, but for some reason the y value is relative to the player
        GameObject gun = this.transform.parent.transform.parent.parent.transform.gameObject;
        hitPoint = gun.GetComponent<GunScript>().hit;

        transform.rotation = hitPoint.transform.rotation;
        transform.position = hitPoint.transform.position;

        Hit.transform.position = hitPoint.point;
        Hit.transform.rotation = Quaternion.LookRotation(hitPoint.normal);

        Hit.Play();

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPoint.point, 0.15f);
        Gizmos.DrawCube(Hit.transform.position, new Vector3(0.3f, 0.3f, 0.3f));
    }
}
// Debug.Log(hit.transform.name);
// TakeDamage target = hit.transform.GetComponent<TakeDamage>();
// gunFlash.Play();
// roundTrace.transform.position = nozzle.transform.position;
// roundTrace.transform.rotation = transform.rotation;
// roundTrace.Play();
// hitEffect.transform.position = hit.point;
// hitEffect.transform.rotation = Quaternion.LookRotation(hit.normal);
// hitEffect.Play();
// Debug.DrawLine(transform.position, hit.point, Color.black, 1);