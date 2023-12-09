using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerCar;
    [SerializeField] GameObject carPrefab;
    PlayerInput playerInput;

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
    }
}
