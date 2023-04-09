using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Sources;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Values")]
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSpeed;
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float falloffMultiplier;


    public float maxBrakeForce;
    public float downforceMultiplier;

    Rigidbody rb;

    [Header("Others")]
    public Transform frontWing, rearWing;

    public GameObject centerOfMass;

    public TextMeshProUGUI speedometer;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        
        rb.centerOfMass = centerOfMass.transform.position;
    }
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void Update()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
        UpdateTexts();
    }

    public void FixedUpdate()
    {
        float motor = ((falloffMultiplier * rb.velocity.magnitude)/(rb.velocity.magnitude - maxSpeed)) + (maxMotorTorque * Input.GetAxis("Vertical"));
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brake = maxBrakeForce * Input.GetAxis("Jump");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            if (axleInfo.brake)
            {
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            }
        }
        rb.AddForce((frontWing.position + -frontWing.up) * downforceMultiplier, ForceMode.Force);
        rb.AddForce((rearWing.position + -rearWing.up) * downforceMultiplier, ForceMode.Force);
    }

    public void UpdateTexts()
    {
        speedometer.text = Mathf.Abs(Mathf.Round(rb.velocity.magnitude)).ToString();
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
    public bool brake; // used in handbrake?
}
