using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] GameObject playerCar;
    [SerializeField] GameObject carPrefab;
    PlayerInput playerInput;

    [SerializeField] bool isTutorial;
    [Space]
    [SerializeField] float MinTimeOnStop;

    [Header("Warnings")]
    public bool isOnCenterLine = false;
    public bool passedStop = false;
    public float TimeSpentOnStop;
    public bool passedRedLight = false;

    [Header("Traffic Violations Counter")]
    [SerializeField] int LightTrafficViolationsCount;
    [SerializeField] int SeriousTrafficViolationsCount;
    [SerializeField] int VerySeriousTrafficViolationsCount;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        //Reset ao carro
        if (playerInput.actions["ResetCar"].triggered)
        {
            Transform currentCarTransform = playerCar.transform;
            Destroy(playerCar.gameObject);
            playerCar = Instantiate(carPrefab, currentCarTransform.position, currentCarTransform.rotation);
        }

        if (isOnCenterLine)
        {
            VerySeriousTrafficViolationsCount++;
            isOnCenterLine = false;
        }

        if (passedStop)
        {
            if (TimeSpentOnStop < MinTimeOnStop)
                SeriousTrafficViolationsCount++;

            passedStop = false;
        }

        if (passedRedLight)
        {
            VerySeriousTrafficViolationsCount++;
            passedRedLight = false;
        }
    }
}
