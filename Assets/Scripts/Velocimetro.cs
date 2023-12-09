using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Velocimetro : MonoBehaviour
{
    public GameObject Vehicle;

    public float maxSpeed = 260.0f;

    public float minSpeedArrowAngle;
    public float maxSpeedArrowAngle;

    [Header("UI")]
    public TMP_Text speedLabel;
    public RectTransform arrow;

    private float speed = 0.0f;

    private void Update()
    {       
        if (Vehicle != null)
        {
            speed = Vehicle.GetComponent<AutoCarController>().speedKMH;
        }
        else
        {
            Vehicle = FindObjectByPartialName("AutomaticCar");
        }

        //Limita a velocidade exibida ao valor máximo (260 km/h)
        speed = Mathf.Min(speed, maxSpeed);

        // Exibe a velocidade arredondada no rótulo.
        if (speedLabel != null)
            speedLabel.text = ((int)speed) + " km/h"; 

        //Rodar a seta do velocimetro
        if (arrow != null)
            arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));
    }

    GameObject FindObjectByPartialName(string partialName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(partialName))
            {
                return obj;
            }
        }

        return null;
    }
}


