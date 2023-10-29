using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string LevelGame;
    [SerializeField] private GameObject initialMenu;
    [SerializeField] private GameObject optionsMenu;

    private float originalButtonPos = 0f;

    private void Start()
    {
        optionsMenu.transform.position = new Vector3(Screen.width / 2, Screen.height + optionsMenu.GetComponent<RectTransform>().sizeDelta.y);

        foreach (Transform button in initialMenu.transform)
        {
            originalButtonPos = button.position.x;

            button.position = new Vector3(-Screen.width, button.position.y);
            button.transform.DOMove(new Vector3(originalButtonPos, button.position.y), 1).SetEase(Ease.OutCubic);
        }
    }

    private IEnumerator TweenButtons(float position, Ease ease, Action function)
    {
        foreach (Transform button in initialMenu.transform)
        {
            button.transform.DOMove(new Vector3(position, button.position.y), 1).SetEase(ease);
        }

        yield return new WaitForSeconds(1);

        if (function != null)
            function.Invoke();
    }

    // M�todo para iniciar o jogo
    public void Play()
    {
        StartCoroutine(TweenButtons(-Screen.width, Ease.InCubic, SwitchToGame));
    }

    private void SwitchToGame()
    {
        SceneManager.LoadScene(LevelGame);
    }

    // M�todo para mostrar o menu de op��es e ocultar o menu inicial
    public void ShowOptions()
    {
        StartCoroutine(TweenButtons(-Screen.width, Ease.InCubic, SwitchToOptions));
    }

    private void SwitchToOptions()
    {
        initialMenu.SetActive(false);
        optionsMenu.SetActive(true);
        optionsMenu.transform.DOMove(new Vector3(Screen.width / 2, Screen.height / 2), 1).SetEase(Ease.OutCubic);
    }

    // M�todo para sair do menu de op��es e voltar ao menu inicial
    public void QuitOptions()
    {
        optionsMenu.transform.DOMove(new Vector3(Screen.width / 2, -Screen.height), 1).SetEase(Ease.InCubic).onComplete = SwitchToInitial;
    }

    private void SwitchToInitial()
    {
        optionsMenu.SetActive(false);
        initialMenu.SetActive(true);
        optionsMenu.transform.position = new Vector3(Screen.width / 2, Screen.height + optionsMenu.GetComponent<RectTransform>().sizeDelta.y);
        StartCoroutine(TweenButtons(originalButtonPos, Ease.OutCubic, null));
    }

    // M�todo para sair do jogo
    public void QuitGame()
    {
        StartCoroutine(TweenButtons(-Screen.width, Ease.InCubic, CloseApplication));
    }

    private void CloseApplication()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

