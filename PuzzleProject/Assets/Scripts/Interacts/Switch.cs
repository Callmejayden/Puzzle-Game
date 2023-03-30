using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Switch : IInteract
{
    
    [SerializeField] private bool defaultOn;


    [SerializeField] private bool isOn;
    public bool IsOn //Only get for other scripts
    {
        get { return this.isOn; }
        set { this.isOn = value; }
    }

    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Door myDoor;
    [SerializeField] private float cycleLength;

    private Vector3 onState;
    private Vector3 offState;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        if (defaultOn)
        {
            IsOn = true;
            //onState = current value
            onState = GameObject.Find("Switch").transform.position;
           
            //offState = current value down 2
            offState = onState;
            offState.y -= .4f;
        }
        else
        {
            IsOn = false;
            //offState = current value
            offState = GameObject.Find("Switch").transform.position;

            //onState = current value up 2
            onState = offState;
            onState.y += .4f;
        }

    }

    public void Toggle(Door door)
    {
        //Change Status
        isOn = !isOn;

        //If lever is getting turned on
        //On:  1, 2, 0.5
        //Off:  1, 0, 0.5
        //(Want to have lever automatically go up and down 2 y)
        if (isOn == true)
        {
            
            transform.DOMove(onState,cycleLength);
        }
        //if lever is getting turned off
        else
        {
            transform.DOMove(offState, cycleLength);

        }

        print("IsOn = "+ isOn);

        ToggleColor(isOn);

        audioSource.Play();

        //If lever is on, door is unlocked 
        door.ToggleLock();
        print("myDoor isLocked = " + myDoor.IsLocked);
    }

    private void ToggleColor(bool isOn)
    {
        if(isOn == true)
        {
            print("Change to on color");
            GetComponent<MeshRenderer>().material = onMaterial;


        }
        else
        {
            print("Change to off color");
            GetComponent<MeshRenderer>().material = offMaterial;

        }
    }

    public override void OnInteract()
    {
        Toggle(myDoor);
        print("Interacted with " + gameObject.name);

    }

    public override void OnFocus()
    {
        print("Looked at " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at " + gameObject.name);
    }

}
