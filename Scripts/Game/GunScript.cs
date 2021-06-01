using UnityEngine;
using MLAPI;

public class GunScript : NetworkBehaviour
{
    public float damage = 10;
    public GameObject nozzle;
    public GameObject particles;
    public RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            shoot();
        }
    }
    void shoot()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit)) //preatty self explanatory, shoots a raycast and shows a debug gizmo
        {
            Debug.Log(hit.transform.name);
            TakeDamage target = hit.transform.GetComponent<TakeDamage>();
            Instantiate(particles, nozzle.transform.position, nozzle.transform.rotation, nozzle.transform);
            Debug.DrawLine(transform.position, hit.point, Color.black, 1);
            if (target != null)
            {
                hit.collider.gameObject.GetComponent<TakeDamage>().damage(damage);
            }
        }
    }
}
