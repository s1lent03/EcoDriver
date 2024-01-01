using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Main References")]
    [SerializeField] Rigidbody sphereRb;
    [SerializeField] PlayerInput playerInput;
    [ReadOnly] Gamepad pad;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform[] Wheels;
    [SerializeField] Transform SteeringWheel;

    [Header("Gear Shift")]
    public int GearNumber; //Gear 0 = Neutral; Gear 1 - 6; Gear 7 = Reverse
    private bool isReverse = false;

    [Header("Gear Velocities")]
    [SerializeField] float gear1MaxVelocity;
    [SerializeField] float gear2MaxVelocity;
    [SerializeField] float gear3MaxVelocity;
    [SerializeField] float gear4MaxVelocity;
    [SerializeField] float gear5MaxVelocity;
    [SerializeField] float gear6MaxVelocity;
    [Space]
    [SerializeField] float minGear1Velocity;
    [SerializeField] float minGear2Velocity;
    [SerializeField] float minGear3Velocity;
    [SerializeField] float minGear4Velocity;
    [SerializeField] float minGear5Velocity;
    [SerializeField] float minGear6Velocity;

    [Header("Speed")]
    [SerializeField] float velocity;
    [SerializeField] float turnSpeed;
    [SerializeField] public float speedKMH;
    [SerializeField] float brake;
    [Space]    
    float moveInput;
    float turnInput;

    [Header("RPM")]
    [SerializeField] float RPM;
    [SerializeField] float finalDriveRatio;
    [SerializeField] float[] gearRatios;
    [SerializeField] float tireCircumference;
    [SerializeField] float axleRatio;

    [Header("Acceleration/Decceleration")]
    [SerializeField] float AccelerationSpeed;
    [SerializeField] float fwdForce;
    [Space]
    [SerializeField] float AccelerationFactor;
    [SerializeField] float DecelerationFactor;

    [Header("Wheels")]
    [SerializeField] float maxWheelsTurnAngle;
    [SerializeField] float wheelsTurnSpeed;
    [SerializeField] float wheelsRotateSpeed;
    [Space]
    [SerializeField] float maxSteeringWheelTurnAngle;

    [Header("Drag")]
    [SerializeField] float airDrag;
    [SerializeField] float groundDrag;

    [Header("Lights")]
    [SerializeField] GameObject leftBreakLight;
    [SerializeField] GameObject rightBreakLight;
    [Space]
    [SerializeField] GameObject leftBlinkerLight;
    [SerializeField] GameObject rightBlinkerLight;
    [SerializeField] float timeBetweenBlinks;
    [Space]
    [SerializeField] GameObject leftHeadLight;
    [SerializeField] GameObject rightHeadLight;

    [Header("Emissions")]
    [SerializeField] float averageEmissions;
    [SerializeField] float minEmissionLimit;
    [SerializeField] float maxEmissionLimit;
    [Space]
    [SerializeField] float totalEmissions;
    [SerializeField] float totalDistance;

    [Header("Bools")]
    [ReadOnly] public bool isGrounded;
    bool leftBlinkerOn = false;
    bool rightBlinkerOn = false;
    bool isBlinking = false;
    bool calculateEmissions = true;


    void Start()
    {
        GearNumber = 0;
        playerInput = GetComponent<PlayerInput>();
        pad = Gamepad.current;

        //Separar esfera do carro
        sphereRb.transform.parent = null;
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
            GearNumber = 7;
        else if (playerInput.actions["Reverse"].triggered && playerInput.actions["Clutch"].ReadValue<float>() == 1 && isReverse == true)
            GearNumber = 1;

        if (GearNumber == 7)
            isReverse = true;
        else
            isReverse = false;

        //Ajustar a rotação do carro
        float newRotation = turnInput * turnSpeed * Time.deltaTime * sphereRb.velocity.magnitude;
        
        if (GearNumber == 7)
            transform.Rotate(0, -newRotation, 0, Space.World);
        else
            transform.Rotate(0, newRotation, 0, Space.World);



        //Ajustar a rotação das rodas
        float wheelsRotationAngle = -90;
        if (moveInput > 0)
            wheelsRotationAngle = turnInput * maxWheelsTurnAngle - 90;

        for (int i = 0; i < 1; i++)
        {
            TurnWheel(Wheels[i], wheelsRotationAngle);
        }

        //Ajustar a rotação do volante
        float steeringWheelRotationAngle = 0;
        if (moveInput > 0)
            steeringWheelRotationAngle = -turnInput * maxSteeringWheelTurnAngle;

        TurnSteeringWheel(SteeringWheel, steeringWheelRotationAngle);

        //Rodar as rodas com base na velocidade do carro
        RotateWheels();

        //PISCAS
        //Esquerda
        if (playerInput.actions["LeftBlinker"].triggered && !leftBlinkerOn)
            leftBlinkerOn = true;
        else if (playerInput.actions["LeftBlinker"].triggered && leftBlinkerOn)
            leftBlinkerOn = false;

        if (leftBlinkerOn)
        {       
            rightBlinkerLight.SetActive(false);
            rightBlinkerOn = false;

            if (!isBlinking)
                StartCoroutine(BlinkLight(leftBlinkerLight));
        }

        //Direita
        if (playerInput.actions["RightBlinker"].triggered && !rightBlinkerOn)
            rightBlinkerOn = true;
        else if (playerInput.actions["RightBlinker"].triggered && rightBlinkerOn)
            rightBlinkerOn = false;

        if (rightBlinkerOn)
        {
            leftBlinkerLight.SetActive(false);
            leftBlinkerOn = false;

            if (!isBlinking)
                StartCoroutine(BlinkLight(rightBlinkerLight));
        }

        //Máximos
        if (playerInput.actions["HeadLights"].IsPressed())
        {
            leftHeadLight.SetActive(true);
            rightHeadLight.SetActive(true);
        }
        else
        {
            leftHeadLight.SetActive(false);
            rightHeadLight.SetActive(false);
        }

        //Posição do carro é sempre igual à da esfera
        transform.position = sphereRb.transform.position;

        //Calcular os RPM
        float speedMS = speedKMH / 3.6f;

        RPM = (speedMS * finalDriveRatio * gearRatios[GearNumber]) / (tireCircumference * axleRatio) * (60 * 60 / 1000);

        //Calcular Emissões
        UpdateTotalEmissionsAndDistance();

        averageEmissions = CalculateAverageEmissions();

        //Ground check
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        if (isGrounded)
            sphereRb.drag = groundDrag;
        else
            sphereRb.drag = airDrag;

        //Calcular velocidade em KM/H
        speedKMH = GetSpeedKMH();
    }

    void FixedUpdate()
    {
        //Ponto de embraiagem
        if (playerInput.actions["Clutch"].ReadValue<float>() > 0.35f && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f && GearNumber == 1 && speedKMH < 10)
        {
            VibrateController(0.2f, 3f);
        }
        else
        {
            StopVibrateController();
        }

        //Ler valor de acelaração / direção
        moveInput = playerInput.actions["Accelerator"].ReadValue<float>();
        turnInput = playerInput.actions["Steer"].ReadValue<Vector2>().x;

        //ACELARAR
        //Acelarar para a frente
        if (GearNumber > 0 && GearNumber <= 6)
        {
            /*Vector3 direction = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            rb.MovePosition(direction + (transform.forward * (velocity * playerInput.actions["Accelerator"].ReadValue<float>()) * Time.deltaTime));*/

            switch (GearNumber)
            {
                case 1:
                    if (speedKMH > minGear1Velocity && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());
                    }
                    else if (playerInput.actions["Clutch"].ReadValue<float>() > 0.35f && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Clutch"].ReadValue<float>() + 0.15f);
                    }
                    break;
                case 2:
                    if (speedKMH > minGear2Velocity && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear2MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());
                    }
                    break;
                case 3:
                    if (speedKMH > minGear3Velocity && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear3MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());
                    }
                    break;
                case 4:
                    if (speedKMH > minGear4Velocity && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear4MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());
                    }
                    break;
                case 5:
                    if (speedKMH > minGear5Velocity && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear5MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());
                    }
                    break;
                case 6:
                    if (speedKMH > minGear6Velocity && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
                    {
                        Accelerate(1, gear6MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());
                    }
                    break;
            }
        }

        //Acelarar para trás
        if (GearNumber == 7)
        {
            if (speedKMH > minGear1Velocity)
            {
                Accelerate(-1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());
            }
            else if (playerInput.actions["Clutch"].ReadValue<float>() > 0.35f && playerInput.actions["Clutch"].ReadValue<float>() < 0.75f)
            {
                Accelerate(-1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Clutch"].ReadValue<float>());
            }
        }

        //TRAVAR
        if (playerInput.actions["Brake"].IsPressed())
        {
            DecelerationFactor = brake;
            if (GearNumber == 7)
            {      
                sphereRb.velocity = transform.forward * (Decelerate() * -1);
            }
            else
            {
                sphereRb.velocity = transform.forward * (Decelerate());
            }

            //Ligar as luzes de travagem
            leftBreakLight.SetActive(true);
            rightBreakLight.SetActive(true);
        }
        else
        {
            DecelerationFactor = 2;

            //Desligar as luzes de travagem
            leftBreakLight.SetActive(false);
            rightBreakLight.SetActive(false);
        }
            

        //Ajustar a velocidade do carro
        //Accelerate(1, gear1MaxVelocity, AccelerationSpeed, playerInput.actions["Accelerator"].ReadValue<float>());

        if (isGrounded)
        {
            //Mover carro
            //sphereRb.AddForce(transform.forward * fwdForce, ForceMode.Acceleration);
        }
        else
        {
            //Aplicar maior gravidade
            sphereRb.AddForce(transform.up * -30f);
        }
    }

    //Faz o carro acelarar
    public void Accelerate(int direction, float maxVelocity, float accelerationSpeed, float accelerationInput)
    {
        //Calcular a velocidade pretendida
        float targetVelocity = accelerationInput * maxVelocity;

        //Interpolar entre a velocidade atual e a pretendida
        float smoothVelocity = 0;
        if (Mathf.Approximately(accelerationInput, 0f))
        {
            //Desacelarar
            smoothVelocity = Decelerate();
        }
        else
        {
            //Acelarar
            smoothVelocity = Mathf.SmoothStep(sphereRb.velocity.magnitude, targetVelocity, Time.fixedDeltaTime * accelerationSpeed * AccelerationFactor);
        }

        //Definir velocidade
        sphereRb.velocity = transform.forward * (smoothVelocity * direction);
    }

    //Faz o carro desacelarar
    public float Decelerate()
    {
        return Mathf.MoveTowards(sphereRb.velocity.magnitude, 0f, Time.fixedDeltaTime * DecelerationFactor);
    }

    void TurnWheel(Transform wheel, float targetRotation)
    {
        float step = wheelsTurnSpeed * Time.deltaTime;
        wheel.localRotation = Quaternion.RotateTowards(wheel.localRotation, Quaternion.Euler(0, targetRotation, 0), step);
    }

    void TurnSteeringWheel(Transform wheel, float targetRotation)
    {
        float step = wheelsTurnSpeed * Time.deltaTime;
        wheel.localRotation = Quaternion.RotateTowards(wheel.localRotation, Quaternion.Euler(0, 0, targetRotation), step);
    }

    void RotateWheels()
    {
        float carSpeed = sphereRb.velocity.magnitude;
        float rotationAmount = carSpeed * wheelsRotateSpeed * Time.deltaTime;

        foreach (Transform wheel in Wheels)
        {
            wheel.Rotate(-Vector3.forward * rotationAmount);
        }
    }

    float GetSpeedKMH()
    {
        //Transforma a velocidade do RB em KM/H
        float speed = sphereRb.velocity.magnitude * 3.6f;
        return speed;
    }

    float GetEstimateEmissions()
    {
        float speedCoefficient = 0.01f;
        float rpmCoefficient = 0.001f;

        float emissions = speedKMH * speedCoefficient + RPM * rpmCoefficient;

        return emissions;
    }

    void UpdateTotalEmissionsAndDistance()
    {
        float estimatedEmissions = GetEstimateEmissions();
        float deltaTime = Time.deltaTime;

        // Update total emissions and total distance
        totalEmissions += estimatedEmissions * deltaTime;
        totalDistance += speedKMH * deltaTime;
    }

    float CalculateAverageEmissions()
    {
        // Ensure total distance is not zero to avoid division by zero
        if (totalDistance > 0.0f)
        {
            return totalEmissions / totalDistance;
        }
        else
        {
            return 0.0f; // or any default value when distance is zero
        }
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

    IEnumerator BlinkLight(GameObject LightToBlink)
    {
        isBlinking = true;

        LightToBlink.SetActive(true);
        yield return new WaitForSeconds(timeBetweenBlinks);
        LightToBlink.SetActive(false);
        yield return new WaitForSeconds(timeBetweenBlinks);

        isBlinking = false;
    }
}
