using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text GearText;
    public GameObject Vehicle;
    public RectTransform FastVectors;

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

        float speed = Vehicle.GetComponent<AutoCarController>().speedKMH;
        FastVectors.sizeDelta = new Vector2(ChangeFastVectorsSize(speed, 0, 260, 4800, 1920), ChangeFastVectorsSize(speed, 0, 260, 2700, 1080));
    }

    public float ChangeFastVectorsSize(float x, float xMin, float xMax, float yMin, float yMax)
    {
        // Clamping the value of x to ensure it stays within the specified range
        float clampedX = Mathf.Clamp(x, xMin, xMax);

        // Performing linear interpolation
        float normalizedX = (clampedX - xMin) / (xMax - xMin);
        float mappedY = (1 - normalizedX) * yMin + normalizedX * yMax;

        return mappedY;
    }
}
