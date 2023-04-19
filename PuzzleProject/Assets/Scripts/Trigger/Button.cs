using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private bool isOn = false;
    public bool IsOn //Only get for other scripts
    { get { return this.isOn; } }

    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Material myMaterial;
    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        myMaterial = this.GetComponent<Material>();
    }

    public void Toggle()
    {
        //Change Status
        isOn = !isOn;

        print("IsOn = "+ isOn);

        ToggleColor(isOn);

        //audioSource.Play();

    }

    private void ToggleColor(bool isOn)
    {
        if(isOn == true)
        {
            print("Change to on color");
            
        }
        else
        {
            print("Change to off color");

        }
    }

}
