using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AniManager : MonoBehaviour
{
    //Var Declarations
    #region Vars
    private Animator anim;
    #endregion


    // -- One major caveat to this "system" is that it reduces the flexibility of running animations due to the inability to add new animations without directly editing the script, 
    // -- and their condition names must be matching that of the script (this could be fixed with a custom editor but despite what it may seem like at time, I do try to maintain *some* ammount of sanity. -Joe


    // Start is called before the first frame update
    void Start()
    {
        //Looks for an animator component in the gameObject and 
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w")) {
            //sets the current animation to walking when the w key is pressed.
            anim.SetBool("isWalking",true);

                //Changes the current animation to running when both w and the shift keys are pressed.
            if (Input.GetKey("left shift")) {
                anim.SetBool("isRunning", true);
            }
            else {
                anim.SetBool("isRunning", false);
            }
        } else {
            anim.SetBool("isWalking", false);
        }
    }
}
