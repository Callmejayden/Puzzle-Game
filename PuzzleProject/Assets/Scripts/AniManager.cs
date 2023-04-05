using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AniManager : MonoBehaviour
{
    //Var Declarations
    #region Vars
    private Animator anim;

    [Header("Settings")]
    private float velocityZ = 0f;
    private float velocityX = 0f;
    [Space]
    public float maxWalkVelocity;
    public float maxRunVelocity;
    [Space]
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float decelDeadZone = 0.05f;

    int ZHash;
    int XHash;
    #endregion

    // -- One major caveat to this "system" is that it reduces the flexibility of running animations due to the inability to add new animations without directly editing the script, 
    // -- and their condition names must be matching that of the script (this could be fixed with a custom editor but despite what it may seem like at time, I do try to maintain *some* ammount of sanity. -Joe

    //Condenced if ladder into two functions, open at own risk
    void changeVelocity(bool wPress, bool aPress, bool dPress, bool sPress, bool sprint, float currentMaxVel) {
        // These if statments are added like this to make "combining" different movements easier, and to prevent bugs
        //Forward movement
        if (wPress && velocityZ < currentMaxVel) {
            velocityZ += Time.deltaTime * acceleration;
        }

        //'Left' Movement
        if (sPress && velocityZ > -currentMaxVel) {
            velocityZ -= Time.deltaTime * acceleration;
        }

        //'Right' Movement
        if (dPress && velocityX < currentMaxVel) {
            velocityX += Time.deltaTime * acceleration;
        }

        //Backward Movement (ideally much slower than forwards or L/R).
        if (aPress && velocityX > -currentMaxVel) {
            velocityX -= Time.deltaTime * acceleration;
        }

        // Z Deceleration
        if (!wPress && velocityZ > 0.0f) {
            velocityZ -= deceleration * Time.deltaTime;
        }

        // X Deceleration / Acceleration (relative to 0)
        if (!dPress && velocityX > 0.0f) {
            velocityX -= deceleration * Time.deltaTime;
        }

        if (!aPress && velocityX < 0.0f) {
            //Despite this 'accelerating' to 0, we still use decelerate here because it has to do with resetting the animation to its base state (or 'idle').
            //Because of this we want to use the same universal multiplier to make said change...
            velocityX += deceleration * Time.deltaTime;
        }
    }
    void velLimiter(bool wPress, bool aPress, bool dPress, bool sPress, bool sprint, float currentMaxVel) {
        //Velocity Resets (so you aren't permenantly walking in one direction.

        // Z Reset (incase Z decelerates below 0
        if (!wPress && !sPress && velocityZ < 0.0f) {
            velocityZ = 0.0f;
        }

        // X Reset
        if (!aPress && !dPress && velocityX != 0.0f && velocityX > -decelDeadZone && velocityX < decelDeadZone) {
            velocityX = 0.0f;
        }

        //Limiting velocity
        //FORWARD
        if (wPress && sprint && velocityZ > currentMaxVel) {
            velocityZ = currentMaxVel;
        }
        else if (wPress && velocityZ > currentMaxVel) {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVel && velocityZ < (currentMaxVel + decelDeadZone)) {
                velocityZ = currentMaxVel;
            }
        }
        else if (wPress && velocityZ < currentMaxVel && velocityZ > (currentMaxVel - decelDeadZone)) {
            velocityZ = currentMaxVel;
        }
        //LEFT
        if (aPress && sprint && velocityX < -currentMaxVel) {
            velocityX = -currentMaxVel;
        }
        else if (aPress && velocityX < -currentMaxVel) {
            velocityX += Time.deltaTime * deceleration;
            if (velocityX < -currentMaxVel && velocityX > (-currentMaxVel - decelDeadZone)) {
                velocityX = -currentMaxVel;
            }
        }
        else if (aPress && velocityX > -currentMaxVel && velocityX < (-currentMaxVel + decelDeadZone)) {
            velocityX = -currentMaxVel;
        }
        //RIGHT
        if (dPress && sprint && velocityX > currentMaxVel) {
            velocityX = currentMaxVel;
        }
        else if (dPress && velocityX > currentMaxVel) {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX > currentMaxVel && velocityX < (currentMaxVel + decelDeadZone)) {
                velocityX = currentMaxVel;
            }
        }
        else if (dPress && velocityX < currentMaxVel && velocityX > (currentMaxVel - decelDeadZone)) {
            velocityX = currentMaxVel;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Looks for an animator component in the gameObject and 
        anim = GetComponent<Animator>();
        //Creating hashes instead of strings for calling the floats in the animation controller to improve performance
        ZHash = Animator.StringToHash("Velocity Z");
        XHash = Animator.StringToHash("Velocity X");
    }

    // Update is called once per frame
    void Update() {
        //Bools check for keypresses at the top of the loop instead of multiple times within. This is for optimization
        bool wPress = Input.GetKey(KeyCode.W);
        bool aPress = Input.GetKey(KeyCode.A);
        bool sPress = Input.GetKey(KeyCode.S);
        bool dPress = Input.GetKey(KeyCode.D);

        bool sprint = Input.GetKey(KeyCode.LeftShift);

            // this acts like an if statement... '?' if (item) then set var to (item) ':' else set var to (item)
        float currentMaxVel = sprint ? maxRunVelocity : maxWalkVelocity;

        changeVelocity(wPress, aPress, dPress, sPress, sprint, currentMaxVel);
        velLimiter(wPress, aPress, dPress, sPress, sprint, currentMaxVel);

        //sets the specefied x/z values to the proper floats in the character animatior
        anim.SetFloat(ZHash, velocityZ);
        anim.SetFloat(XHash, velocityX);
    }
}
