using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    //Movement Variables
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    [SerializeField]
    private bool invertedMouse = false;
    [SerializeField]
    private float thrusterForce = 10000f;

    //Thruster Fuel Variables
    [SerializeField]
    private float thrusterFuelBurnSpeed=500f;
    [SerializeField]
    private float thrusterRegainSpeed = 200f;
    private float thrusterFuelAmount = 1f;

    // Joint Variables
    [Header("Spring Setting")]
    [SerializeField]
    private float jointSpring=20f;
    [SerializeField]
    private float maxSpringForce = 40f;
    private ConfigurableJoint configurableJoint;
    [SerializeField]
    private LayerMask walkableLayer;


    private float disableAtTime =0f;

    private PlayerMotor motor;   
    private Animator animator;
    private string animatorFloatName = "ForwardVelocity";



    // Use this for initialization
    void Start () {
        motor = GetComponent<PlayerMotor>();
        configurableJoint = GetComponent<ConfigurableJoint>();
        SetJointSetting(jointSpring);
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        //calculat movement velocity
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;
        //Apply Animation
        animator.SetFloat(animatorFloatName, _zMov);
        //Apply movement

        motor.Move(_velocity);

        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        motor.Rotate(_rotation);


        float _xRot = Input.GetAxisRaw("Mouse Y");
        if(!invertedMouse)
        {
            _xRot = -_xRot;
        }
        _xRot*= lookSensitivity;
        motor.CameraRotate(_xRot);

        //Apply the thruster force
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= Time.deltaTime * thrusterFuelBurnSpeed;
            if(thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSetting(0f);
            }
            
        }
        else
        {
            thrusterFuelAmount += Time.deltaTime * thrusterRegainSpeed;
            thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);
            SetJointSetting(jointSpring);
        }
        motor.ApplyThruster(_thrusterForce);

    }

    void FixedUpdate()
    {
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 10f,walkableLayer))
        {
            configurableJoint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            Debug.Log("Didn't Hit anything");
            configurableJoint.targetPosition = Vector3.zero;
        } 
    }
    private void SetJointSetting(float _jointSpring)
    {
        configurableJoint.yDrive = new JointDrive{
                                                    positionSpring = _jointSpring,
                                                    maximumForce =maxSpringForce
                                                 };
    }
    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }
    public void OnDisable()
    {
        //Stop Movement
        motor.Move(Vector3.zero);

        // Animation
        animator.SetFloat(animatorFloatName, 0f);


        //Stop thrust
        motor.ApplyThruster(Vector3.zero);

        //Enable Spring
        SetJointSetting(jointSpring);

        //Stop Camera Rotation
        motor.Rotate(Vector3.zero);
        motor.CameraRotate(0f);

        //Save current Time
        disableAtTime = Time.realtimeSinceStartup;


    }
    public void OnEnable()
    {
        //Fill the fuel as compensation when the script was disabled
        if (disableAtTime != 0f)
        {
            thrusterFuelAmount += (Time.realtimeSinceStartup - disableAtTime) * thrusterRegainSpeed;
            thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);
        }        
    }
}
