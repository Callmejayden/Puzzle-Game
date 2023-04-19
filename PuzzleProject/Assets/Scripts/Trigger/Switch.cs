using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Trigger
{
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
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

    //Is called once Activate() is called
    public override void Change()
    {
        ToggleColor(isOn);
        audioSource.Play();
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
