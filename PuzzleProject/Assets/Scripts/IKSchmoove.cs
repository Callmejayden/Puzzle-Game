
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class IKSchmoove : MonoBehaviour
{
    //VARS -- Sorted first by privacy then by type (privates on top, and publics in their respective categories [because that determines how they appear in the inspector]). -- with the exception of serializedFields
    #region vars
    private float inputX;
    private float inputZ;
    private float speed;
    private float prevPelvPos, RPrevFootPos, LPrevFootPos;
    private float vertVelocity;

    private Vector3 moveTo;
    private Vector3 RfootPos, LfootPos;
    private Vector3 RIkPos, LIkPos;
    private Vector3 moveVector;

    private Quaternion RIkRot, LIkRot;

    private bool grounded;
    private bool rotationLock = false;

    public GameObject cam;
    public Animator anim;
    public CharacterController charController;

    [Range(0, 0.5f)] public float allowedRot = 0.1f;
    [Range(0, 0.5f)] public float rotSpeed = 0.1f;

    [Header("ANIMATION SETTINGS -- *unimplemented*")]
    [Space]
    [Range(0, 1f)] public float XSmoothTime = 0.2f;
    [Range(0, 1f)] public float zSmoothTime = 0.2f;
    [Range(0, 1f)] public float startTime = 0.3f;
    [Range(0, 1f)] public float stopTime = 0.15f;


    [Header("FEET FINDER")]
    [Space]
    public bool enableFeetIk = true;
    [Space]
    [SerializeField] private LayerMask enviroment;
    [SerializeField] private float pelvOffset = 0f;
    [Space]
    [Range(0,2)][SerializeField] private float heightFromBase = 1.14f;
    [Range(0, 2)][SerializeField] private float rayDistance = 1.5f;
    [Range(0, 1)][SerializeField] private float pelvBobbingRate = 0.28f;
    [Range(0, 1)][SerializeField] private float footTransitionSpeed = 0.5f;
    [Space]
    public string LFootVarName = "LeftFootCurve";
    public string RFootVarName = "RightFootCurve";
    [Space]
    public bool useCustomFeatures = false;
    public bool showDebug = true;

    #endregion

    #region __init__
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        //cam = Camera.main;
        charController = this.GetComponent<CharacterController>();

        if(anim == null) {
            Debug.LogError(transform.name + " is required to have an animator for IK script to function.");
        }
    }
    #endregion
    #region updates
    // Update is called once per frame
    void Update()
    {
        //InputMagnitude();
    }
    #endregion

    #region movement
    /*
    void PlayerMove() {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        //var camera = Camera.main;
        var camera = cam;
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.Normalize();
        right.Normalize();

        moveTo = forward * inputZ + right * inputX;

        if(rotationLock == false) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveTo), rotSpeed);
        }
    }

   void InputMagnitude() {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        anim.SetFloat("InputZ", inputZ, zSmoothTime, Time.deltaTime * 2f);
        anim.SetFloat("InputX", inputX, XSmoothTime, Time.deltaTime * 2f);

        speed = new Vector2(inputX, inputZ).sqrMagnitude;

        if (speed > allowedRot) {
            anim.SetFloat("InputMagnitude", speed, startTime, Time.deltaTime);
            PlayerMove();
        }
        
        else if (speed < allowedRot) {
            anim.SetFloat("InputMagnitude", speed, stopTime, Time.deltaTime);
        }
    }
   */
    #endregion

    #region Grounding
    /// <summary>
    /// Updates the 'AdjustFootTarget' method and grabs the location of both feet.
    /// </summary>
    private void FixedUpdate() {
        //Checks to prevent crashes
        if(enableFeetIk == false) { return; }
        if(anim == null) { return; }

        //Runs the 'AdjustFootTarget' method for both the left and right foot
        AdjustFootTarget(ref RfootPos, HumanBodyBones.RightFoot);
        AdjustFootTarget(ref LfootPos, HumanBodyBones.LeftFoot);

        //Sends out raycast towards ground to find "where the ground be" (for both feet ofc).
        FootSolver(RfootPos, ref RIkPos, ref RIkRot);
        FootSolver(LfootPos, ref LIkPos, ref LIkRot);
    }

    private void OnAnimatorIK(int layerIndex) {
        //Checks to prevent crashes
        if (enableFeetIk == false) { return; }
        if(anim == null) { return; }

        CharBobing();

        //Right foot schmoover -- uses "custom features"
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        if (useCustomFeatures) {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(RFootVarName));
        }

        FeetToIk(AvatarIKGoal.RightFoot, RIkPos, RIkRot, ref RPrevFootPos);

        //Left foot schmoover -- uses "custom features"
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        if (useCustomFeatures) {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(LFootVarName));
        }

        FeetToIk(AvatarIKGoal.LeftFoot, LIkPos, LIkRot, ref LPrevFootPos);
    }
    #endregion

    #region groundingMethods

    /// <summary>
    /// Moves feet to IK position.
    /// </summary>
    /// <param name="foot"></param>
    /// <param name="tempIkPos"></param>
    /// <param name="tempIkRot"></param>
    /// <param name="prevFootPosY"></param>
    void FeetToIk(AvatarIKGoal foot, Vector3 tempIkPos, Quaternion tempIkRot, ref float prevFootPosY) {

        Vector3 targetIkPos = anim.GetIKPosition(foot);

        if(tempIkPos != Vector3.zero) {
            targetIkPos = transform.InverseTransformPoint(targetIkPos);
            tempIkPos = transform.InverseTransformPoint(tempIkPos);

            float yPos = Mathf.Lerp(prevFootPosY, tempIkPos.y, footTransitionSpeed);
            targetIkPos.y += yPos;

            prevFootPosY = yPos;
            targetIkPos = transform.TransformPoint(targetIkPos);

            anim.SetIKRotation(foot, tempIkRot);
        }

        anim.SetIKPosition(foot, targetIkPos);
    }

    /// <summary>
    /// Changes height of pelvis "bobing"
    /// </summary>
    private void CharBobing() {
        if(RIkPos == Vector3.zero || LIkPos == Vector3.zero || prevPelvPos == 0) { 
            prevPelvPos = anim.bodyPosition.y;
            return;
        }

        float leftOffsetPos = LIkPos.y - transform.position.y;
        float rightOffsetPos = RIkPos.y - transform.position.y;
        float totalOffset = (leftOffsetPos < rightOffsetPos) ? leftOffsetPos : rightOffsetPos;

        Vector3 newPelvPos = anim.bodyPosition + Vector3.up * totalOffset;
        newPelvPos.y = Mathf.Lerp(prevPelvPos, newPelvPos.y, pelvBobbingRate);

        anim.bodyPosition = newPelvPos;
        prevPelvPos = anim.bodyPosition.y;
    }
  
    /// <summary>
    /// Locates new position for feet via raycasts
    /// </summary>
    /// <param name="fromSkyPos"></param>
    /// <param name="footIkPos"></param>
    /// <param name="footIkRot"></param>
    private void FootSolver(Vector3 fromSkyPos, ref Vector3 feetIkPos, ref Quaternion feetIkRot) {
        //Racasts baby! -- line below creates raycast and stores hit info
        RaycastHit hit;
        //Shows debuglines if the toggle in inspector is checked
        if (showDebug) { Debug.DrawLine(fromSkyPos, fromSkyPos + Vector3.down * (rayDistance + heightFromBase), Color.yellow); }

        if (Physics.Raycast(fromSkyPos, Vector3.down, out hit, rayDistance + heightFromBase, enviroment)) {
            feetIkPos = fromSkyPos;
            feetIkPos.y = hit.point.y + pelvOffset;
            feetIkRot = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            return;
        }

        feetIkPos = Vector3.zero; //it no workie
    }

    /// <summary>
    /// Moves Feet Target.
    /// </summary>
    /// <param name="feetPos"></param>
    /// <param name="foot"></param>
    private void AdjustFootTarget (ref Vector3 feetPos, HumanBodyBones foot) {
        feetPos = anim.GetBoneTransform(foot).position;
        feetPos.y = transform.position.y + heightFromBase;
    }
    #endregion
}
