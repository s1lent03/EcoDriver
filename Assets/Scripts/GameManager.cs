using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject playerCar;
    [SerializeField] GameObject carPrefab;
    PlayerInput playerInput;
    [Space]
    [SerializeField] GameObject InfoWindow;
    public bool isWindowActive = true;

    [SerializeField] bool isTutorial;
    [Space]
    [SerializeField] public float MinTimeOnStop;

    [Header("Warnings")]
    public bool isOnCenterLine = false;
    public bool passedStop = false;
    public float TimeSpentOnStop;
    public bool passedRedLight = false;
    public int maxVelocityInArea = 50;
    public bool canCheckVelocity = true;

    [Header("Traffic Violations Counter")]
    [SerializeField] public int LightTrafficViolationsCount;
    [SerializeField] public int SeriousTrafficViolationsCount;
    [SerializeField] public int VerySeriousTrafficViolationsCount;
    [Space]
    [SerializeField] TMP_Text LightTrafficViolationsTxt;
    [SerializeField] TMP_Text SeriousTrafficViolationsTxt;
    [SerializeField] TMP_Text VerySeriousTrafficViolationsTxt;
    [Space]
    [SerializeField] public int passedStopSign;
    [SerializeField] public int passedCentralLine;
    [SerializeField] public int passedTrafficLight;
    [SerializeField] public int passedOverSpeedLimit;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();     
    }

    void Update()
    {
        //Reset ao carro
        /*if (playerInput.actions["ResetCar"].triggered)
        {
            Transform currentCarTransform = playerCar.transform;
            Destroy(playerCar.gameObject);
            playerCar = Instantiate(carPrefab, currentCarTransform.position, currentCarTransform.rotation);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }*/

        if (isOnCenterLine)
        {
            VerySeriousTrafficViolationsCount++;
            StartCoroutine(UpdateInfo("MuitoGrave"));
            isOnCenterLine = false;
            passedCentralLine++;
        }

        if (passedStop)
        {
            if (TimeSpentOnStop < MinTimeOnStop)
            {
                SeriousTrafficViolationsCount++;
                StartCoroutine(UpdateInfo("Grave"));
                passedStopSign++;
            }          
            
            passedStop = false;
        }

        if (passedRedLight)
        {
            VerySeriousTrafficViolationsCount++;
            StartCoroutine(UpdateInfo("MuitoGrave"));
            passedRedLight = false;
            passedTrafficLight++;
        }

        if (playerInput.actions["ShowInfo"].triggered)
            ShowWindow(isWindowActive);

        if (canCheckVelocity && playerCar.GetComponent<CarController>().speedKMH > maxVelocityInArea)
        {
            if (playerCar.GetComponent<CarController>().speedKMH - maxVelocityInArea <= 20)
            {
                LightTrafficViolationsCount++;
                StartCoroutine(UpdateInfo("Leve"));
            }
                

            if (playerCar.GetComponent<CarController>().speedKMH - maxVelocityInArea > 20 && playerCar.GetComponent<CarController>().speedKMH - maxVelocityInArea <= 40)
            {
                SeriousTrafficViolationsCount++;
                StartCoroutine(UpdateInfo("Grave"));
            }
                

            if (playerCar.GetComponent<CarController>().speedKMH - maxVelocityInArea > 40)
            {
                VerySeriousTrafficViolationsCount++;
                StartCoroutine(UpdateInfo("MuitoGrave"));
            }

            passedOverSpeedLimit++;
            canCheckVelocity = false;
        }
    }

    IEnumerator UpdateInfo(string typeOfViolation)
    {
        if (typeOfViolation == "Leve")
        {
            float elapsedTime = 0f;
            float lerpDuration = 0.5f;

            while (elapsedTime < lerpDuration)
            {
                // Incrementally change the font size over time
                LightTrafficViolationsTxt.fontSize = Mathf.Lerp(36, 60, elapsedTime / lerpDuration);

                // Update the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }

            elapsedTime = 0f;
            LightTrafficViolationsTxt.text = LightTrafficViolationsCount.ToString();

            while (elapsedTime < lerpDuration)
            {
                // Incrementally change the font size over time
                LightTrafficViolationsTxt.fontSize = Mathf.Lerp(60, 36, elapsedTime / lerpDuration);

                // Update the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }            
        }
        else if (typeOfViolation == "Grave")
        {
            float elapsedTime = 0f;
            float lerpDuration = 0.5f;

            while (elapsedTime < lerpDuration)
            {
                // Incrementally change the font size over time
                SeriousTrafficViolationsTxt.fontSize = Mathf.Lerp(36, 60, elapsedTime / lerpDuration);

                // Update the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }

            elapsedTime = 0f;
            SeriousTrafficViolationsTxt.text = SeriousTrafficViolationsCount.ToString();

            while (elapsedTime < lerpDuration)
            {
                // Incrementally change the font size over time
                SeriousTrafficViolationsTxt.fontSize = Mathf.Lerp(60, 36, elapsedTime / lerpDuration);

                // Update the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }            
        }
        else if (typeOfViolation == "MuitoGrave")
        {
            float elapsedTime = 0f;
            float lerpDuration = 0.5f;

            while (elapsedTime < lerpDuration)
            {
                // Incrementally change the font size over time
                VerySeriousTrafficViolationsTxt.fontSize = Mathf.Lerp(36, 60, elapsedTime / lerpDuration);

                // Update the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }

            elapsedTime = 0f;
            VerySeriousTrafficViolationsTxt.text = VerySeriousTrafficViolationsCount.ToString();

            while (elapsedTime < lerpDuration)
            {
                // Incrementally change the font size over time
                VerySeriousTrafficViolationsTxt.fontSize = Mathf.Lerp(60, 36, elapsedTime / lerpDuration);

                // Update the elapsed time
                elapsedTime += Time.deltaTime;

                // Wait for the next frame
                yield return null;
            }            
        }
    }

    void ShowWindow(bool isActive)
    {
        
        if (isActive)
        {
            isWindowActive = false;
            InfoWindow.transform.DOLocalMoveX(-1240f, 1f);            
        }
        else
        {
            isWindowActive = true;
            InfoWindow.transform.DOLocalMoveX(-650f, 1f);            
        }
        
    }
}
