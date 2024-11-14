using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Animator MyAnim;
    private Rigidbody MyRigid;
    private float MouseY_Raw;
    private float MouseX_Raw;
    private float RawHorizontal;
    private float RawVertical;
    private float RotX;
    internal GameObject MainCam;

    internal bool IsAiming;
    internal bool IsEmoting;


    [Header("Refs")]
    public GameObject CamAttachment;
    public GameObject CamHelperOrigin;
    public GameObject CamHelper;

    [Header("Movements")]
    public float MoveSpeed;
    public float JumpHeight;

    [Header("Rotations")]
    public float TurnSpeed;
    public float RotAngle;
    public bool ShouldAlsoRotateChar;
    public GameObject Spine;
    public Vector3 SpineOffset;

    [Header("Cam Collisions")]
    public LayerMask TriggerMask;
    public float Cam_Offset;

    [Header("Aiming")]
    public float AimingFOV;
    public float Non_AimingFOV;
    public GameObject Reticle;
    public LayerMask ShootingLayerMask;
    public float Length;
    public Color ReticleNormal;
    public Color ReticleShoot;

    void Start()
    {
        MyAnim = GetComponent<Animator>();
        MyRigid = GetComponent<Rigidbody>();
        MainCam = Camera.main.gameObject;
        Reticle.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MouseY_Raw = -Input.GetAxisRaw("Mouse Y");
        MouseX_Raw = Input.GetAxisRaw("Mouse X");

        RawHorizontal = Input.GetAxisRaw("Horizontal");
        RawVertical = Input.GetAxisRaw("Vertical");

        // Rotate both character & camera
        CharacterAndCamRotations();

        // This is to move our character
        CharacterMovement();

        // This is for animating the character
        CharacterAnimations();

        // This is our wall hack - UE Spring Arm Component
        CamWallCollisionHelper();

        // Aiming Feature
        Aiming();

        // Jumping
        Jumping();

        // Emoting
        Emoting();

    }

    private void LateUpdate()
    {
        if (IsAiming)
        {
            Spine.transform.Rotate(SpineOffset);
        }
    }

    private void Aiming()
    {
        IsAiming = Input.GetMouseButton(1);
        MyAnim.SetBool("AIM", IsAiming);

        MainCam.GetComponent<Camera>().fieldOfView = (IsAiming) ? AimingFOV : Non_AimingFOV;
        Reticle.SetActive(IsAiming);

        bool ShouldChangeColor = false;
        RaycastHit hit;
#if UNITY_EDITOR
        Debug.DrawRay(MainCam.gameObject.transform.position,
            MainCam.gameObject.transform.forward, Color.red);

        Debug.DrawLine(MainCam.gameObject.transform.position,            //1 Origin
            MainCam.gameObject.transform.position +                      //2
            (MainCam.gameObject.transform.forward * Length), Color.green);
#endif
        if (Physics.Linecast(MainCam.gameObject.transform.position,      //1 Origin
            MainCam.gameObject.transform.position +                      //2
            (MainCam.gameObject.transform.forward * Length)              //2 Destination (Endpoint)
            , out hit,                                                 // 3 where to store the hit info
            ShootingLayerMask))                                        // 4 what layers to query
        {
#if UNITY_EDITOR
            print("1");
#endif 
            if (hit.transform)
            {
                if (hit.transform.CompareTag("Shootable"))
                {
#if UNITY_EDITOR
                    print("3");
#endif 
                    ShouldChangeColor = true;
                }
            }
        }

        Reticle.GetComponent<Image>().color =
            (!ShouldChangeColor) ? ReticleNormal : ReticleShoot;
    }

    private void CharacterAnimations()
    {
        MyAnim.SetFloat("Horiz", RawHorizontal);
        MyAnim.SetFloat("Vert", RawVertical);
        MyAnim.SetFloat("MoveSpeed", Mathf.Max(Mathf.Abs(RawVertical), Mathf.Abs(RawHorizontal)));
    }

    private void CamWallCollisionHelper()
    {
        MainCam.transform.localPosition = Vector3.zero;
        RaycastHit hit;
        if (Physics.Linecast(
            CamHelperOrigin.transform.position,
            CamHelper.transform.position,
            out hit,
            TriggerMask))
        {
            if (hit.transform)
            {
                Vector3 Dir =
                    CamHelperOrigin.transform.position - hit.point;
                MainCam.transform.position =
                    hit.point + (Dir * Cam_Offset)
                    + (hit.normal * Cam_Offset);
            }
        }
    }

    private void CharacterAndCamRotations()
    {
        RotX += MouseY_Raw * TurnSpeed;
        RotX = Mathf.Clamp(RotX, -RotAngle, RotAngle);
        Quaternion localRot =
            Quaternion.Euler(-RotX, CamAttachment.transform.eulerAngles.y, 0f);

        CamAttachment.transform.rotation = localRot;

        Transform TransformToRot = null;
        if (ShouldAlsoRotateChar)
        {
            TransformToRot = transform;
            Quaternion localRot2 = Quaternion.Euler(RotX, Spine.transform.eulerAngles.y, 0f);
            Spine.transform.rotation = localRot2;
        }
        else
        {
            TransformToRot = CamAttachment.transform;
        }

        TransformToRot.Rotate(new Vector3(0, TurnSpeed * MouseX_Raw, 0), Space.Self);
    }

    private void CharacterMovement()
    {
        Vector3 MoveVec = GetMovementVector();
        MoveVec = MoveVec.normalized * MoveSpeed;

        MyRigid.velocity = new Vector3(MoveVec.x, MyRigid.velocity.y, MoveVec.z);
    }

    private Vector3 GetMovementVector()
    {
        Vector3 MoveVec = Vector3.zero;
        MoveVec += transform.forward * RawVertical;
        MoveVec += transform.right * RawHorizontal;
        return MoveVec;
    }

    private void Jumping()
    {
        Vector3 MoveVec = GetMovementVector();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyAnim.SetBool("Jumping", true);
            MyRigid.velocity = new Vector3(MoveVec.x, JumpHeight, MoveVec.z);
            MyAnim.SetBool("Jumping", false);
        }
    }

    private void Emoting()
    {
        IsEmoting = Input.GetKeyDown(KeyCode.P);
        MyAnim.SetBool("Emoting", IsEmoting);
    }
}
