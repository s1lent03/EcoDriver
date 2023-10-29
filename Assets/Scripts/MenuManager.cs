using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string LevelGame;
    [SerializeField] private GameObject initialMenu;
    [SerializeField] private GameObject optionsMenu;

    // Método para iniciar o jogo
    public void Play()
    {
        SceneManager.LoadScene(LevelGame);
    }

    // Método para mostrar o menu de opções e ocultar o menu inicial
    public void ShowOptions()
    {
        initialMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Método para sair do menu de opções e voltar ao menu inicial
    public void QuitOptions()
    {
        optionsMenu.SetActive(false);
        initialMenu.SetActive(true);
    }

    // Método para sair do jogo
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

