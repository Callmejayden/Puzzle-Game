using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GravityGunScript GravityGunScript;
    public Rigidbody Rigid;
    public BoxCollider Collider;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    public void Start()
    {
        // setup 
        if (!equipped)
        {
            GravityGunScript.enabled = false;
            Rigid.isKinematic = false;
            Collider.isTrigger = false;
        }
        if (equipped)
        {
            GravityGunScript.enabled = true;
            Rigid.isKinematic = true;
            Collider.isTrigger = true;
            slotFull = true;
        }
    }
    private void Update()
    {   // check if player is in range and E is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        // drop if equipped and Q is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();

    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        // make wapon a child of the camera and move it to default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        // make rigidbody kinematic and box collider a trigger 
        Rigid.isKinematic = true;
        Collider.isTrigger = true;

        // enable script
        GravityGunScript.enabled = true;

    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        // set parent to null
        transform.SetParent(null);



        // make rigidbody kinematic and box collider a trigger 
        Rigid.isKinematic = false;
        Collider.isTrigger = false;

        // gun carries momentume of player
        Rigid.velocity = player.GetComponent<Rigidbody>().velocity;

        // add force
        Rigid.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        Rigid.AddForce(fpsCam.up * dropForwardForce, ForceMode.Impulse);

        // add random rotaion 
        float random = Random.Range(-1f, 1f);
        Rigid.AddTorque(new Vector3(random, random, random) * 10);

        // enable script
        GravityGunScript.enabled = false;
    }
}
