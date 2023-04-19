using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoerDoor : MonoBehaviour, IDo
{
    
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool canBeInteractedWith = true;
    private Animator anim;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    //This door opens
    public void Do()
    {

            //toggles from true to false
            isOpen = !isOpen;

            print("Do " + gameObject.name);

            Vector3 doorTransformDirection = transform.TransformDirection(Vector3.forward);


            Vector3 playerTransformDirection = FirstPersonControll.instance.transform.position - transform.position;

            float dot = Vector3.Dot(doorTransformDirection, playerTransformDirection);

            print("dot is" + dot);

            anim.SetFloat("dot", dot);
            anim.SetBool("isOpen", isOpen);

        
    }

    public void OnFocus()
    { }

    public void OnLoseFocus()
    { }

}
