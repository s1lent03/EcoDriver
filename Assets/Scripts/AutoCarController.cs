using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class AutoCarController : MonoBehaviour
{
    [Header ("Main References")]
    PlayerInput playerInput;
    private Gamepad pad;
    private Rigidbody rb;

    [Header("Gear Shift")]   
    public int GearNumber; //Gear 0 = Neutral; Gear 1 - 5; Gear 6 = Reverse
    private bool isReverse = false;


    [Header("Velocity/Acceleration")]
    public float velocity = 10f;      
    [Space]
    public float gear1MaxVelocity;
    public float gear2MaxVelocity;
    public float gear3MaxVelocity;
    public float gear4MaxVelocity;
    public float gear5MaxVelocity;
    [Space]
    public float AccelerationSpeed;
    public float decelerationFactor;
    [Space]
    public float minGear1Velocity;
    public float minGear2Velocity;
    public float minGear3Velocity;
    public float minGear4Velocity;
    public float minGear5Velocity;
    [Space]
    public float speedKMH;
    public float accelerationFactor;
    private Vector3 lastPosition;

    [Header("Steer")]
    public float steerVelocity;
    private float steerDirection = 0f;
    private Vector3 steerTurn;

    [Header("Gravity")]
    public float gravityForce = 9.8f;

    void Start()
    {
        GearNumber = 0;
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        pad = Gamepad.current;

        lastPosition = transform.position;
    }

    void Update()
    {
        //Se a embraiagem tiver a fundo sobe uma mudança
        if (playerInput.actions["UpGear"].triggered && GearNumber < 5 && playerInput.actions["Clutch"].ReadValue<float>() == 1)
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
        if (playerInput.actions["Clutch"].ReadValue<float>() > 0.35f && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f && GearNumber == 1)
        {
            VibrateController(0.2f, 3f);
        }
        else
        {
            StopVibrateController();
        }

        //GRAVIDADE
        ApplyGravity();

        //TRAVAR
        if (playerInput.actions["Brake"].IsPressed())
        {
            if (GearNumber == 6)
            {
                decelerationFactor = 6;
                rb.velocity = transform.forward * (Decelerate() * -1);
            }
            else
            {
                decelerationFactor = 6;
                rb.velocity = transform.forward * (Decelerate());
            }
        }
        else
            decelerationFactor = 2;

        //ACELARAR
        //Acelarar para a frente
        if (GearNumber > 0 && GearNumber < 6)
        {
            /*Vector3 direction = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rb.MovePosition(direction + (transform.forward * (velocity * playerInput.actions["Accelerator"].ReadValue<float>()) * Time.deltaTime));*/

            switch (GearNumber)
            {
                case 1:
                    if (speedKMH > minGear1Velocity && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());

                        //Controlar vibração
                        float vibrationLvl = CustomNormalize(speedKMH, minGear1Velocity, minGear2Velocity);
                        VibrateController(vibrationLvl, vibrationLvl);
                    }
                    else if (playerInput.actions["Clutch"].ReadValue<float>() > 0.35f && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Clutch"].ReadValue<float>() + 0.15f);
                    }                    
                    break;
                case 2:
                    if (speedKMH > minGear2Velocity /*&& playerInput.actions["Clutch"].ReadValue<float>() < 0.75f*/)
                    {
                        Accelerate(1, gear2MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());

                        //Controlar vibração
                        float vibrationLvl = CustomNormalize(speedKMH, minGear2Velocity, minGear3Velocity);
                        VibrateController(vibrationLvl, vibrationLvl);
                    }                    
                    break;
                case 3:
                    if (speedKMH > minGear3Velocity /*&& playerInput.actions["Clutch"].ReadValue<float>() < 0.75f*/)
                    {
                        Accelerate(1, gear3MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());

                        //Controlar vibração
                        float vibrationLvl = CustomNormalize(speedKMH, minGear3Velocity, minGear4Velocity);
                        VibrateController(vibrationLvl, vibrationLvl);
                    }                    
                    break;
                case 4:
                    if (speedKMH > minGear4Velocity /*&& playerInput.actions["Clutch"].ReadValue<float>() < 0.75f*/)
                    {
                        Accelerate(1, gear4MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());

                        //Controlar vibração
                        float vibrationLvl = CustomNormalize(speedKMH, minGear4Velocity, minGear5Velocity);
                        VibrateController(vibrationLvl, vibrationLvl);
                    }                    
                    break;
                case 5:
                    if (speedKMH > minGear5Velocity /*&& playerInput.actions["Clutch"].ReadValue<float>() < 0.75f*/)
                    {
                        Accelerate(1, gear5MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());

                        //Controlar vibração
                        float vibrationLvl = CustomNormalize(speedKMH, minGear5Velocity, 120f);
                        VibrateController(vibrationLvl, vibrationLvl);
                    }                   
                    break;
            }
        }

        //Acelarar para trás
        if (GearNumber == 6)
        {
            /*Vector3 direction = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rb.MovePosition(direction - (transform.forward * (velocity * playerInput.actions["Accelerator"].ReadValue<float>()) * Time.deltaTime));*/

            if (speedKMH > minGear1Velocity)
            {
                Accelerate(-1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());

                //Controlar vibração
                float vibrationLvl = CustomNormalize(speedKMH, minGear1Velocity, minGear2Velocity);
                VibrateController(vibrationLvl, vibrationLvl);
            }
            else if (playerInput.actions["Clutch"].ReadValue<float>() > 0.35f && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
            {
                Accelerate(-1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Clutch"].ReadValue<float>());
            }
        }

        //DIREÇÃO
        //Direção para a frente
        if (playerInput.actions["Steer"].IsPressed() && GearNumber > 0 && GearNumber < 6 && speedKMH > 0)
        {
            Vector2 move = playerInput.actions["Steer"].ReadValue<Vector2>();

            steerDirection = move.x * steerVelocity * (velocity * (playerInput.actions["Accelerator"].ReadValue<float>() + 1));
            steerTurn = new Vector3(transform.rotation.x, steerDirection, transform.rotation.z);
            transform.Rotate(steerTurn * Time.deltaTime);
        }
        else
            steerDirection = 0;

        //Direção para a trás
        if (playerInput.actions["Steer"].IsPressed() && GearNumber == 6 && speedKMH > 0)
        {
            Vector2 move = playerInput.actions["Steer"].ReadValue<Vector2>();

            steerDirection = move.x * steerVelocity * (velocity * playerInput.actions["Accelerator"].ReadValue<float>());
            steerTurn = new Vector3(transform.rotation.x, -steerDirection, transform.rotation.z);
            transform.Rotate(steerTurn * Time.deltaTime);
        }
        else
            steerDirection = 0;

        //Velocidade do veículo
        Vector3 currentPosition = transform.position;
        Vector3 displacement = currentPosition - lastPosition;

        float speedMS = displacement.magnitude / Time.deltaTime;
        speedKMH = speedMS * 3.6f;

        lastPosition = currentPosition;  
    }

    void ApplyGravity()
    {
        // Simulate gravity by adjusting the car's position
        Vector3 gravityVector = Vector3.down * gravityForce;
        transform.position += gravityVector * Time.deltaTime;
    }

    //Normalizar valores
    public float CustomNormalize(float value, float min, float max)
    {
        // Ensure that the value is clamped between min and max
        value = Mathf.Clamp(value, min, max);

        // Perform linear interpolation
        float normalizedValue = (value - min) / (max - min);

        return normalizedValue;
    }

    //Faz o carro acelarar
    public void Accelerate(int direction, float maxVelocity, float accelerationSpeed, float accelerationInput)
    {
        //Calcular a velocidade pretendida
        float targetVelocity = accelerationInput * maxVelocity;

        //Interpolar entre a velocidade atual e a pretendida
        float smoothVelocity;
        if (Mathf.Approximately(accelerationInput, 0f))
        {
            //Desacelarar
            smoothVelocity = Decelerate();
        }
        else
        {
            //Acelarar
            smoothVelocity = Mathf.SmoothStep(rb.velocity.magnitude, targetVelocity, Time.fixedDeltaTime * accelerationSpeed * accelerationFactor);
        }

        //Definir velocidade
        rb.velocity = transform.forward * (smoothVelocity * direction);        
    }

    //Faz o carro desacelarar
    public float Decelerate()
    {
        return Mathf.MoveTowards(rb.velocity.magnitude, 0f, Time.fixedDeltaTime * decelerationFactor);
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
