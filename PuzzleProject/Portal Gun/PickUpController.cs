using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public PortalGun gunScript1;
    
    public Rigidbody body;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Start()
    {
        if (!equipped)
        {
            gunScript1.enabled = false;
            
            body.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gunScript1.enabled = true;
           
            body.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }
    private void Update()
    {
        Vector3 distaceToPlayer = player.position - transform.position;
        if (!equipped && distaceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        body.isKinematic = true;
        coll.isTrigger = true;

        gunScript1.enabled = true;
        
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        transform.SetParent(null);

        body.isKinematic = false;
        coll.isTrigger = false;

        body.velocity = player.GetComponent<Rigidbody>().velocity;

        body.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        body.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        body.AddTorque(new Vector3(random, random, random) * 10);

        gunScript1.enabled = false;
       
    }
}

