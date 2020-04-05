using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Controller_Rover_system : System.Object
{




    [Header("------------WHEELS BASE SETUP------------")]

    [Tooltip("If  true - Use Torque in this wheels")]
    public bool ThisMotor;


    [Tooltip("If  true - reverse this wheels")]
    public bool Reverse;

    [Tooltip("If  true - this wheel's can steering")]
    public bool ThisSteering;



    [Tooltip("If _ThisSteereng == true - reverse steering angle")]
    public bool SteeringRevers;


    [Header("------------  WHEELS COLLIDER  ------------")]
    [Tooltip("Left Wheel Collider")]
    public WheelCollider Left_Wheel_Collider;
    [Tooltip("Right Wheel Collider")]
    public WheelCollider Right_Wheel_Collider;
    [Header("------------     WHEELS MESH      ------------")]
    [Tooltip("Left Wheel mesh")]
    public GameObject Left_Wheel_Mesh;
    [Tooltip("Right Wheel Mesh")]
    public GameObject Right_Wheel_Mesh;

}

public class Controller_Rover : MonoBehaviour
{



    [Header("WHEELS SETUP")]
    public List<Controller_Rover_system> Wheel_Settings;

    [Header("ENGINE POWER SETTINGS")]

    [Tooltip("Maximum engine power. If more - faster acceleration and torque.If less - slower and weaker. For power per speed control - use torque curve")]
    [Range(16.0f, 8192.0f)]
    public float MaxPower = 750f;

    [Tooltip("Maximum speed in KPH. For power per speed control - use torque curve")]
    [Range(10.0f, 220.0f)]
    public float MaxSpeed = 45f;
    [Tooltip("Torque curve - needed for power per speed control. to example - big power in small speed  - helpful for hillclimbing,  big power in hight speed - good for sportcars")]
    public AnimationCurve Torque_Curve;

    [Header("STEER SETTINGS")]

    [Range(0.1f, 55f)]



    [Tooltip("Wheels steering - maximum angle in Low speed")]
    public float MaxAngle = 45f;
    [Range(0.1f, 40.0f)]


    [Tooltip("Wheels steering - maximum angle in hight speed")]
    public float MaxAngle_InSpeed = 8.0f;
    [Range(-1.2f, 1.2f)]

    [Tooltip("Differential balance")]
    public float Differential_Power = 0.1f;

    [Header("BODY SETTINGS")]
    public Vector3 CenterOfMass_offset;

    public Rigidbody RoverRigidBody;
    private bool Sleep_Rigidbody_Debug;


    [Header("VEHICLE DRIVING DATA")]

    [Tooltip("Current torque")]
    public float FinalPower;
    [Tooltip("Current speed in unity engine units")]
    public float CurrentSpeed;

    [Tooltip("Current speed in KPH")]
    public float CurrentSpeed_KPH;

    [Tooltip("Current speed in MPH")]
    public float CurrentSpeed_MPH;
    [Tooltip("RPM counter")]
    public float WheelRPM;

    [Tooltip("Wheel for RPM counter")]
    public WheelCollider RPM_counter_collider;



    private float Torque_Curve_Lerp;
    private float SleepDelay = 3f;
    private float SleepDelay_current;
    private float TorqueCalc;




    public void Wheel_Render(Controller_Rover_system Wheel_proces)
    {

        Vector3 Wheel_position;
        Quaternion Wheel_rotation;

         

        Wheel_proces.Left_Wheel_Collider.GetWorldPose(out Wheel_position, out Wheel_rotation);
        Wheel_proces.Left_Wheel_Mesh.transform.position = Wheel_position;
        Wheel_proces.Left_Wheel_Mesh.transform.rotation = Wheel_rotation;

       


        Wheel_proces.Right_Wheel_Collider.GetWorldPose(out Wheel_position, out Wheel_rotation);
        Wheel_proces.Right_Wheel_Mesh.transform.position = Wheel_position;
        Wheel_proces.Right_Wheel_Mesh.transform.rotation = Wheel_rotation;
        
    }









    public void FixedUpdate()
    {

        CurrentSpeed = RoverRigidBody.velocity.magnitude;

        if (CurrentSpeed < 0.09f & Mathf.Abs(Input.GetAxis("Vertical")) < 0.15f)
        {
            SleepDelay_current += Time.fixedDeltaTime;

            if (SleepDelay_current > SleepDelay)
            {
                RoverRigidBody.Sleep();
                WheelRPM = 0.0f;
                CurrentSpeed_KPH = 0.0f;
                CurrentSpeed_MPH = 0.0f;
            }

        }
        else
        {
            SleepDelay_current = 0.0f;
            WheelRPM = RPM_counter_collider.rpm;
            CurrentSpeed_KPH = CurrentSpeed * 3.6f;
            CurrentSpeed_MPH = CurrentSpeed * 2.23f;
        }


        Sleep_Rigidbody_Debug = RoverRigidBody.IsSleeping();







        Torque_Curve_Lerp = Mathf.Clamp01(CurrentSpeed_KPH / MaxSpeed);

        if (CurrentSpeed_KPH < MaxSpeed)
        {
            TorqueCalc = MaxPower * -1 * Input.GetAxis("Vertical") * Torque_Curve.Evaluate(Torque_Curve_Lerp);
        }

        else
        {
            TorqueCalc = 0.0f;
        }



        float motor = TorqueCalc;
        float ThisSteering = Mathf.Lerp(MaxAngle, MaxAngle_InSpeed, Torque_Curve_Lerp) * Input.GetAxis("Horizontal");


        RoverRigidBody.centerOfMass = CenterOfMass_offset;

        float brakePower = Mathf.Abs(Input.GetAxis("Jump"));
        if (brakePower > 0.0015f)
        {
            brakePower = MaxPower;
            motor = 0;
        }
        else
        {
            brakePower = 0;
        }

        foreach (Controller_Rover_system Wheel_proces in Wheel_Settings)
        {



           
            if (Wheel_proces.ThisSteering == true)
            {
                if (Wheel_proces.SteeringRevers == false)
                {
                    Wheel_proces.Left_Wheel_Collider.steerAngle = Wheel_proces.Right_Wheel_Collider.steerAngle = ((Wheel_proces.Reverse) ? -1 : 1) * ThisSteering;
                }

                else
                {
                    Wheel_proces.Left_Wheel_Collider.steerAngle = Wheel_proces.Right_Wheel_Collider.steerAngle = ((Wheel_proces.Reverse) ? -1 : 1) * ThisSteering * -1.0f;
                }
            }

            if (Wheel_proces.ThisMotor == true)
            {

                if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f)
                {
                    Wheel_proces.Left_Wheel_Collider.motorTorque = motor;
                    Wheel_proces.Right_Wheel_Collider.motorTorque = motor;
                }

                else
                {
                    if (Input.GetAxis("Horizontal") > 0.15f)
                    {


                        Wheel_proces.Left_Wheel_Collider.motorTorque = motor;
                        Wheel_proces.Right_Wheel_Collider.motorTorque = motor * Differential_Power;
                    }

                    if (Input.GetAxis("Horizontal") < -0.15f)
                    {
                        Wheel_proces.Left_Wheel_Collider.motorTorque = motor * Differential_Power;
                        Wheel_proces.Right_Wheel_Collider.motorTorque = motor;
                    }

                }

            }

            Wheel_proces.Left_Wheel_Collider.brakeTorque = brakePower;
            Wheel_proces.Right_Wheel_Collider.brakeTorque = brakePower;



            FinalPower = Wheel_proces.Left_Wheel_Collider.motorTorque;


            Wheel_Render(Wheel_proces);
        }





    }




}





