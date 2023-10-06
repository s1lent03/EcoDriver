using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AutoCarController : MonoBehaviour
{
    [Header("Main Vals")]
    PlayerInput playerInput;
    public int GearNumber; //Gear 0 = Neutral; Gear 1 - 5; Gear 6 = Reverse

    [Header("Button Press Checks")]
    private bool isUpGearPressed = false;
    private bool isDownGearPressed = false;

    void Start()
    {
        GearNumber = 0;
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (playerInput.actions["UpGear"].IsPressed() && !isUpGearPressed && GearNumber < 6)
        {
            isUpGearPressed = true;
            GearNumber += 1;
        }

        if (!playerInput.actions["UpGear"].IsPressed())
            isUpGearPressed = false;

        if (playerInput.actions["DownGear"].IsPressed() && !isDownGearPressed && GearNumber > 0)
        {
            isDownGearPressed = true;
            GearNumber -= 1;
        }

        if (!playerInput.actions["DownGear"].IsPressed())
            isDownGearPressed = false;
    }
}
