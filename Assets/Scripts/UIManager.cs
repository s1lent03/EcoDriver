using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text GearText;
    public GameObject Vehicle;

    private int gearNumber;

    void Start()
    {
        
    }

    void Update()
    {
        gearNumber = Vehicle.GetComponent<AutoCarController>().GearNumber;

        if (gearNumber > 0 && gearNumber < 6)
            GearText.text = gearNumber.ToString();

        if (gearNumber == 0)
            GearText.text = "N";

        if (gearNumber == 6)
            GearText.text = "R";
    }
}
