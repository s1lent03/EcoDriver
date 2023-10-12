using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AutoCarController : MonoBehaviour
{
    [Header ("Main References")]
    PlayerInput playerInput;
    private Gamepad pad;
    private Rigidbody rb;

    [Header("Main Vals")]   
    public int GearNumber; //Gear 0 = Neutral; Gear 1 - 5; Gear 6 = Reverse

    [Header("Velocity")]
    public float velocity = 10f;
    public float maxVelocity = 200f;

    [Header("Steer")]
    public float steerVelocity = 10f;
    private float steerDirection = 0f;
    private Vector3 steerTurn;

    void Start()
    {
        GearNumber = 0;
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        pad = Gamepad.current;
    }

    void Update()
    {
        //Se a embraiagem tiver a fundo sobe uma mudança
        if (playerInput.actions["UpGear"].triggered && GearNumber < 6 && playerInput.actions["Clutch"].ReadValue<float>() == 1)
            GearNumber += 1;

        //Se a embraiagem tiver a fundo desce uma mudança
        if (playerInput.actions["DownGear"].triggered && GearNumber > 0 && playerInput.actions["Clutch"].ReadValue<float>() == 1)
            GearNumber -= 1;

    }

    void FixedUpdate()
    {
        //Ponto de embraiagem
        if (playerInput.actions["Clutch"].ReadValue<float>() > 0.35f && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
        {
            VibrateController(0.25f, 0.5f);
        }
        else
        {
            StopVibrateController();
        }

        //Acelarar
        if (GearNumber > 0 && GearNumber < 6)
        {
            Vector3 direction = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rb.MovePosition(direction + (transform.forward * (velocity * playerInput.actions["Accelerator"].ReadValue<float>()) * Time.deltaTime));
        }

        //Direção
        if (playerInput.actions["Steer"].IsPressed() && GearNumber > 0 && GearNumber < 6)
        {
            Vector2 move = playerInput.actions["Steer"].ReadValue<Vector2>();

            steerDirection += move.x * steerVelocity;
            steerTurn = new Vector3(transform.rotation.x, transform.rotation.y + steerDirection, transform.rotation.z);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(steerTurn), 0.75f * Time.deltaTime);
            //transform.rotation = Quaternion.Euler(steerTurn);
            //Debug.Log(steerDirection);
        }
        else
            steerDirection = 0;
    }

    //Faz o comando vibrar
    public void VibrateController(float lowFrequency, float highFrequency)
    {
        if (pad != null)
        {
            pad.SetMotorSpeeds(lowFrequency, highFrequency);
        }
    }

    //Pára a vibração do comando
    public void StopVibrateController()
    {
        if (pad != null)
        {
            pad.SetMotorSpeeds(0, 0);
        }
    }
}
