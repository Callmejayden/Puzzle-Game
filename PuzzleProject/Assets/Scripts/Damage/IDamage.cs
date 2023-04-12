using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDamage : MonoBehaviour
{
    //Changes every IDamage to layer 9
    public virtual void Awake()
    {
        gameObject.tag = "IDamage";
    }


    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}
