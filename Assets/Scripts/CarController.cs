using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    PlayerInput playerInput;
    public float maxRPM = 7000.0f;
    public float maxTorque = 500.0f;
    public float brakeTorque = 1000.0f;
    public float steerVelocity = 2.0f;
    public float wheelRadius = 0.5f; // Adjust based on your car
    public float gearRatio = 3.0f;   // Adjust based on your car's gearing
    private bool isReverse = false;

    private Rigidbody rb;
    private float velocity;
    private float currentRPM;
    public int gearNumber = 0; // 0 is neutral, 1-5 are forward gears, 6 is reverse
    private float currentTorque = 0.0f;


    public float torqueMultiplier;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        // Handle gear changes
        HandleGearChanges();

        // Calculate RPM and update gearNumber based on your logic
        UpdateRPM();

        // Smoothly accelerate/decelerate based on RPM
        SmoothAcceleration();

        // Handle gear shifting logic
        //HandleGearShifting();

        // Apply torque to the wheels
        ApplyTorque();
    }

    void HandleGearChanges()
    {
        //Se a embraiagem tiver a fundo sobe uma mudança
        if (playerInput.actions["UpGear"].triggered && gearNumber < 6 && playerInput.actions["Clutch"].ReadValue<float>() == 1)
            gearNumber += 1;

        //Se a embraiagem tiver a fundo desce uma mudança
        if (playerInput.actions["DownGear"].triggered && gearNumber > 0 && playerInput.actions["Clutch"].ReadValue<float>() == 1)
            gearNumber -= 1;

        //Troca mais facil entre qualquer mudança e reverse. Caso esteja já em reverse, passa para 1ª
        if (playerInput.actions["Reverse"].triggered && playerInput.actions["Clutch"].ReadValue<float>() == 1 && isReverse == false)
            gearNumber = 6;
        else if (playerInput.actions["Reverse"].triggered && playerInput.actions["Clutch"].ReadValue<float>() == 1 && isReverse == true)
            gearNumber = 1;

        if (gearNumber == 6)
            isReverse = true;
        else
            isReverse = false;
    }

    void UpdateRPM()
    {
        // Calculate RPM based on linear velocity
        float linearVelocity = rb.velocity.magnitude;
        currentRPM = CalculateRPM(linearVelocity);
    }

    float CalculateRPM(float linearVelocity)
    {
        // You'll need to calibrate this formula based on your specific setup
        // This is a simplified example and may not provide precise results for all cases

        // Convert linear velocity to RPM
        float wheelCircumference = 2 * Mathf.PI * wheelRadius;
        float wheelRPM = linearVelocity / wheelCircumference * 60.0f;

        // Adjust for gearing and other factors
        float adjustedRPM = wheelRPM * gearRatio;

        // Clamp the RPM to a maximum value (optional)
        adjustedRPM = Mathf.Clamp(adjustedRPM, 0.0f, maxRPM);

        return adjustedRPM;
    }

    void SmoothAcceleration()
    {
        // Use Mathf.Lerp or another smoothing method to adjust torque smoothly based on RPM
        float targetTorque = CalculateTargetTorque() * torqueMultiplier;
        currentTorque = Mathf.Lerp(currentTorque, targetTorque, Time.deltaTime * 5.0f);
    }

    void HandleGearShifting()
    {
        // Adjust these values based on your game's requirements
        float shiftUpRPM = 0.9f * maxRPM; // RPM threshold to shift up
        float shiftDownRPM = 0.2f * maxRPM; // RPM threshold to shift down

        // Shift up if RPM is above the threshold and not already in the highest gear
        if (currentRPM > shiftUpRPM && gearNumber < 6)
        {
            gearNumber += 1;
        }
        // Shift down if RPM is below the threshold and not already in neutral or reverse
        else if (currentRPM < shiftDownRPM && gearNumber > 1)
        {
            gearNumber -= 1;
        }
    }

    float CalculateTargetTorque()
    {
        // Adjust this function based on your desired acceleration curve
        float normalizedRPM = currentRPM / maxRPM;
        float targetTorque = Mathf.Lerp(0.0f, maxTorque, normalizedRPM);

        return targetTorque;
    }

    void ApplyTorque()
    {
        // Apply torque to the wheels based on the currentTorque value and gearNumber
        float torque = currentTorque * playerInput.actions["Accelerator"].ReadValue<float>(); // Adjust for your input axis

        // Reverse if in reverse gear
        if (gearNumber == 6)
            torque = -torque;

        Debug.Log("Torque: " + torque);

        rb.AddRelativeForce(Vector3.forward * torque);
    }
}
