using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum WarningType
{
    Leve,
    Grave,
    MtGrave,
}

public class Notification : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Image WarningIcon;
    [SerializeField] TMP_Text WarningText;
    [Space]
    [SerializeField] Color WarningColorLight;
    [SerializeField] Color WarningColorSerious;
    [SerializeField] Color WarningColorVerySerious;
    [Space]
    [SerializeField] float OffScreenPositionX;
    [SerializeField] float OnScreenPositionX;
    [Space]
    [SerializeField] float timeOnScreen;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void ReceiveNotification(string info, WarningType typeWarning)
    {
        if (typeWarning == WarningType.Leve)
        {
            StartCoroutine(DoNotification(info, WarningColorLight));
        }
        else if(typeWarning == WarningType.Grave)
        {
            StartCoroutine(DoNotification(info, WarningColorSerious));
        }
        else if (typeWarning == WarningType.MtGrave)
        {
            StartCoroutine(DoNotification(info, WarningColorVerySerious));
        }
    }

    IEnumerator DoNotification(string notiDesc, Color warningColor)
    {
        gameObject.transform.DOLocalMoveX(OnScreenPositionX, 1);

        WarningIcon.color = warningColor;
        WarningText.text = notiDesc;

        yield return new WaitForSeconds(timeOnScreen);
        
        DestroySelf();
    }

    public void DestroySelf()
    {
        gameObject.transform.DOLocalMoveX(OffScreenPositionX, 1);
        Destroy(gameObject);
    }

}
