using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string LevelGame;
    [SerializeField] private GameObject initialMenu;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject fade;

    [Header("Sounds")]
    public AudioSource buttonClickSoundFX;

    private float originalButtonPos = 0f;

    private void Start()
    {
        audioMenu.transform.position = new Vector3(Screen.width / 2, Screen.height + audioMenu.GetComponent<RectTransform>().sizeDelta.y);

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

    // Método para iniciar o jogo
    public void Play()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();
        StartCoroutine(TweenButtons(-Screen.width, Ease.InCubic, SwitchToGame));
    }

    private void SwitchToGame()
    {
        SceneManager.LoadScene(LevelGame);
    }

    // Método para mostrar o menu de gráficos e ocultar o menu inicial
    public void ShowAudio()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();
        StartCoroutine(TweenButtons(-Screen.width, Ease.InCubic, SwitchToAudio));
    }

    private void SwitchToAudio()
    {
        initialMenu.SetActive(false);
        audioMenu.SetActive(true);
        fade.SetActive(true);
        audioMenu.transform.DOMove(new Vector3(Screen.width / 2, Screen.height / 2), 1).SetEase(Ease.OutCubic);
    }

    // Método para sair dos menus de opções e voltar ao menu inicial
    public void QuitOptions()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();
        audioMenu.transform.DOMove(new Vector3(Screen.width / 2, -Screen.height), 1).SetEase(Ease.InCubic).onComplete = SwitchToInitial;
    }

    private void SwitchToInitial()
    {
        initialMenu.SetActive(true);
        audioMenu.SetActive(false);
        fade.SetActive(false);
        audioMenu.transform.position = new Vector3(Screen.width / 2, Screen.height + audioMenu.GetComponent<RectTransform>().sizeDelta.y);
        StartCoroutine(TweenButtons(originalButtonPos, Ease.OutCubic, null));
    }

    // Método para sair do jogo
    public void QuitGame()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();
        StartCoroutine(TweenButtons(-Screen.width, Ease.InCubic, CloseApplication));
    }

    private void CloseApplication()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

