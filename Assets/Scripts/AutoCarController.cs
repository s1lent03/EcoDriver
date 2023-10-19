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

    [Header("Gear Shift")]   
    public int GearNumber; //Gear 0 = Neutral; Gear 1 - 5; Gear 6 = Reverse
    private bool isReverse = false;

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

        //Troca mais facil entre qualquer mudança e reverse. Caso esteja já em reverse, passa para 1ª
        if (playerInput.actions["Reverse"].triggered && playerInput.actions["Clutch"].ReadValue<float>() == 1 && isReverse == false)
            GearNumber = 6;
        else if (playerInput.actions["Reverse"].triggered && playerInput.actions["Clutch"].ReadValue<float>() == 1 && isReverse == true)
            GearNumber = 1;

        if (GearNumber == 6)
            isReverse = true;
        else
            isReverse = false;
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

        //ACELARAR
        //Acelarar para a frente
        if (GearNumber > 0 && GearNumber < 6)
        {
            Vector3 direction = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rb.MovePosition(direction + (transform.forward * (velocity * playerInput.actions["Accelerator"].ReadValue<float>()) * Time.deltaTime));
        }

        //Acelarar para trás
        if (GearNumber == 6)
        {
            Vector3 direction = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rb.MovePosition(direction - (transform.forward * (velocity * playerInput.actions["Accelerator"].ReadValue<float>()) * Time.deltaTime));
        }

        //DIREÇÃO
        //Direção para a frente
        if (playerInput.actions["Steer"].IsPressed() && GearNumber > 0 && GearNumber < 6 && !(playerInput.actions["Accelerator"].ReadValue<float>() == 0))
        {
            Vector2 move = playerInput.actions["Steer"].ReadValue<Vector2>();

            steerDirection = move.x * steerVelocity * (velocity * playerInput.actions["Accelerator"].ReadValue<float>());
            steerTurn = new Vector3(transform.rotation.x, steerDirection, transform.rotation.z);
            transform.Rotate(steerTurn * Time.deltaTime);
        }
        else
            steerDirection = 0;

        //Direção para a trás
        if (playerInput.actions["Steer"].IsPressed() && GearNumber == 6 && !(playerInput.actions["Accelerator"].ReadValue<float>() == 0))
        {
            Vector2 move = playerInput.actions["Steer"].ReadValue<Vector2>();

            steerDirection = move.x * steerVelocity * (velocity * playerInput.actions["Accelerator"].ReadValue<float>());
            steerTurn = new Vector3(transform.rotation.x, -steerDirection, transform.rotation.z);
            transform.Rotate(steerTurn * Time.deltaTime);
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
