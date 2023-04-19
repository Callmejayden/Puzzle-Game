using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damage : MonoBehaviour
{
    //Changes every IDamage to layer 9
    public virtual void Awake()
    {
        gameObject.tag = "Damage";
    }


    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}
