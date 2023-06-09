using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGunScript : MonoBehaviour
{
    public Transform floatPoint;

    public float launchSpeed;

    private Camera cam;

    private GameObject target;

    private Rigidbody targetRig;

    public float weaponRange = 12f;
    private bool isAttracting;
    private bool isLaunching;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            isAttracting = true;
        else if (Input.GetButtonUp("Fire1"))
            isAttracting = false;

        if (isAttracting)
        {
            if (Input.GetButtonDown("Fire2"))
                isLaunching = true;
        }
    }

    private void FixedUpdate()
    {
        if (isAttracting)
            Attract();
        else if (!isAttracting)
            Release();

        if (isLaunching)
            Throw();
    }

    private void Attract()
    {
        RaycastHit hit;
        if (target == null)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weaponRange))
            {
                target = hit.transform.gameObject;
                if (target.tag.Equals("CanGrab"))
                {



                    targetRig = target.GetComponent<Rigidbody>();

                    target.transform.position =
                        Vector3.MoveTowards(target.transform.position, floatPoint.position, 0.3f);
                    targetRig.useGravity = false;
                }
                else
                {
                    target = null;
                }
            }
        }
        else
        {
            target.transform.position = Vector3.MoveTowards(target.transform.position, floatPoint.position, 0.3f);
        }
    }

    private void Release()
    {
        if (target != null)
        {
            targetRig.useGravity = true;
            target = null;
        }
    }

    private void Throw()
    {
        if (targetRig != null)
        {
            targetRig.useGravity = true;
            targetRig.AddForce(floatPoint.transform.forward * launchSpeed, ForceMode.Impulse);
            target = null;
            isLaunching = false;
        }
    }
}