using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndInfoScreen : MonoBehaviour
{
    [Header("Main Vars")]
    [SerializeField] TMP_Text contraLeve;
    [SerializeField] TMP_Text contraGrave;
    [SerializeField] TMP_Text contraMtGrave;
    [Space]
    [SerializeField] TMP_Text numStops;
    [SerializeField] TMP_Text numVermelhos;
    [SerializeField] TMP_Text numLinhas;
    [SerializeField] TMP_Text numVelocidade;
    [Space]
    [SerializeField] TMP_Text numEmissoes;
    [SerializeField] TMP_Text numDistancia;
    [SerializeField] TMP_Text numEmissoesMedia;
    [Space]
    [SerializeField] GameManager manager; 
    [SerializeField] CarController playerCar;


    // Update is called once per frame
    void Update()
    {
        contraLeve.text = manager.LightTrafficViolationsCount.ToString();
        contraGrave.text = manager.SeriousTrafficViolationsCount.ToString();
        contraMtGrave.text = manager.VerySeriousTrafficViolationsCount.ToString();

        numStops.text = manager.passedStopSign.ToString();
        numVermelhos.text = manager.passedTrafficLight.ToString();
        numLinhas.text = manager.passedCentralLine.ToString();
        numVelocidade.text = manager.passedOverSpeedLimit.ToString();

        numEmissoes.text = playerCar.totalEmissions.ToString();
        numDistancia.text = playerCar.totalDistance.ToString();
        numEmissoesMedia.text = playerCar.averageEmissions.ToString();
    }
}
