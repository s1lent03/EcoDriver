using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string LevelGame;
    [SerializeField] private GameObject initialMenu;
    [SerializeField] private GameObject optionsMenu;

    // M�todo para iniciar o jogo
    public void Play()
    {
        SceneManager.LoadScene(LevelGame);
    }

    // M�todo para mostrar o menu de op��es e ocultar o menu inicial
    public void ShowOptions()
    {
        initialMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // M�todo para sair do menu de op��es e voltar ao menu inicial
    public void QuitOptions()
    {
        optionsMenu.SetActive(false);
        initialMenu.SetActive(true);
    }

    // M�todo para sair do jogo
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

