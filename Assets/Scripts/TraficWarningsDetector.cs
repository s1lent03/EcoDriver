using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TraficWarningsDetector : MonoBehaviour
{
    [Header("Object Type")]
    [SerializeField] bool IsCentralLine;
    [SerializeField] bool IsStopSign;
    [SerializeField] bool IsTrafficLight;
    bool canCarGo;
    float EnterStopTime;

    [Header("Traffic Lights")]
    [SerializeField] GameObject TopRedLight;
    [SerializeField] GameObject TopYellowLight;
    [SerializeField] GameObject TopGreenLight;
    [Space]
    [SerializeField] GameObject MidRedLight;
    [SerializeField] GameObject MidGreenLight;
    [Space]
    [SerializeField] GameObject BotRedLight;
    [SerializeField] GameObject BotYellowLight;
    [SerializeField] GameObject BotGreenLight;
    [Space]
    bool canRunChangeAgain = true;
    [SerializeField] bool invertSignal;

    [Header("Others")]
    [SerializeField] GameObject Managers;
    GameManager gameManager;

    void Start()
    {
        Managers = FindObjectWithPartialName("Managers");
        gameManager = Managers.GetComponent<GameManager>();
    }

    void Update()
    {
        if (IsTrafficLight && canRunChangeAgain)
        {
            if (!invertSignal)
                StartCoroutine(ChangeTrafficLights1());
            else
                StartCoroutine(ChangeTrafficLights2());
        }
            
    }

    IEnumerator ChangeTrafficLights1()
    {
        canRunChangeAgain = false;

        TopRedLight.SetActive(true);
        TopYellowLight.SetActive(false);
        TopGreenLight.SetActive(false);

        MidRedLight.SetActive(false);
        MidGreenLight.SetActive(true);

        BotRedLight.SetActive(true);
        BotYellowLight.SetActive(false);
        BotGreenLight.SetActive(false);

        canCarGo = false;

        yield return new WaitForSeconds(8);

        TopRedLight.SetActive(false);
        TopYellowLight.SetActive(false);
        TopGreenLight.SetActive(true);

        MidRedLight.SetActive(true);
        MidGreenLight.SetActive(false);

        BotRedLight.SetActive(false);
        BotYellowLight.SetActive(false);
        BotGreenLight.SetActive(true);

        canCarGo = true;

        yield return new WaitForSeconds(8);

        TopRedLight.SetActive(false);
        TopYellowLight.SetActive(true);
        TopGreenLight.SetActive(false);

        MidRedLight.SetActive(true);
        MidGreenLight.SetActive(false);

        BotRedLight.SetActive(false);
        BotYellowLight.SetActive(true);
        BotGreenLight.SetActive(false);

        yield return new WaitForSeconds(3);

        canRunChangeAgain = true;
    }

    IEnumerator ChangeTrafficLights2()
    {
        canRunChangeAgain = false;

        TopRedLight.SetActive(false);
        TopYellowLight.SetActive(false);
        TopGreenLight.SetActive(true);

        MidRedLight.SetActive(true);
        MidGreenLight.SetActive(false);

        BotRedLight.SetActive(false);
        BotYellowLight.SetActive(false);
        BotGreenLight.SetActive(true);

        canCarGo = true;

        yield return new WaitForSeconds(8);

        TopRedLight.SetActive(false);
        TopYellowLight.SetActive(true);
        TopGreenLight.SetActive(false);

        MidRedLight.SetActive(true);
        MidGreenLight.SetActive(false);

        BotRedLight.SetActive(false);
        BotYellowLight.SetActive(true);
        BotGreenLight.SetActive(false);

        yield return new WaitForSeconds(3);

        TopRedLight.SetActive(true);
        TopYellowLight.SetActive(false);
        TopGreenLight.SetActive(false);

        MidRedLight.SetActive(false);
        MidGreenLight.SetActive(true);

        BotRedLight.SetActive(true);
        BotYellowLight.SetActive(false);
        BotGreenLight.SetActive(false);

        canCarGo = false;

        yield return new WaitForSeconds(8);

        canRunChangeAgain = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (IsCentralLine)
                gameManager.isOnCenterLine = true;

            if (IsStopSign)
                EnterStopTime = Time.time;
        }     
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (IsStopSign)
            {
                gameManager.passedStop = true;
                gameManager.TimeSpentOnStop = Time.time - EnterStopTime;
            }

            if (IsTrafficLight && !canCarGo)
                gameManager.passedRedLight = true;
        }
    }

    //Encontrar objecto com um certo nome
    GameObject FindObjectWithPartialName(string partialName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        //Procurar pelo objecto Manager
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
