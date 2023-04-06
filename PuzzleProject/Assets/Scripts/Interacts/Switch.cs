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

    [SerializeField] private Vector3 onState;
    [SerializeField] private Vector3 offState;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        
        if (defaultOn)
        {
            IsOn = true;

        }
        else
        {
            IsOn = false;
        }
    }

    public void Toggle(Door door)
    {
        //Change Status
        isOn = !isOn;

        //if lever is getting turned on
        if (isOn == true)
        {
            
            Vector3 NewPosition = this.transform.position;
            NewPosition.y += -0.2f;
            transform.DOMove(NewPosition,cycleLength);
        }
        //if lever is getting turned off
        else
        {
            Vector3 NewPosition = this.transform.position;
            NewPosition.y += 0.2f;
            transform.DOMove(NewPosition, cycleLength);

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
