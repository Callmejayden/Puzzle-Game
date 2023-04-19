using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger : Interact
{ 
    //Changes every IInteract to layer 9
    public virtual void Awake()
    {
        this.gameObject.layer = 9;
    }

    [SerializeField] public IDo Doer;

    [SerializeField] public bool isOn = false;

    //If it is a trigger, oninteract becomes activate
    public override void OnInteract()
    { 
        Activate();
    }
    public abstract void Change();
    public void Activate()
    {
        //Base switches isOn and Doer.Do()
        isOn = !isOn;
        Doer.Do();

        Change();
    }

}
