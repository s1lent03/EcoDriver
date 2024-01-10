using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("MenusGameobjects")]
    public GameObject pauseMenu;
    public GameObject audioMenu;
    [Space]
    public GameObject eventSystemObject;
    private GameObject lastSelectedObject;

    [Header("Sounds")]
    public AudioSource navegationSoundFX;
    public AudioSource buttonClickSoundFX;

    [Header("Others")]
    [SerializeField] private PlayerInput playerInput;
    public bool isPaused = false;

    void Start()
    {
        //Atualiza os valores dos mixers
        audioMenu.GetComponent<AudioMenuManager>().UpdateMixersBasedOnFile();

        //Dar um valor default às variaveis
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        //Parar o jogo ou recomeçar
        if (playerInput.actions["Pause"].triggered && !isPaused)
        {
            //Abre o menu
            pauseMenu.SetActive(true);
            isPaused = true;
            Time.timeScale = 0;

            //Faz o cursor aparecer
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (playerInput.actions["Pause"].triggered && isPaused == true)
        {
            ResumeButton();
        }

        //Apenas produzir som caso o jogo esteja em pausa
        if (isPaused == true)
        {
            //Sempre que o utilizador clicar num botão para navegar vai produzir um sound effect
            if (lastSelectedObject != eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject)
            {
                navegationSoundFX.Play();
                lastSelectedObject = eventSystemObject.GetComponent<EventSystem>().currentSelectedGameObject;
            }
        }        
    }

    //Volta ao jogo
    public void ResumeButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Esconde o menu
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;

        //Faz o cursor desaparecer
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Abre o menu do audio
    public void AudioButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Setup à janela do audio
        audioMenu.GetComponent<AudioMenuManager>().UpdateSlidersBasedOnFile();

        //Esconde o menu pausa e abre o menu dos visuals
        pauseMenu.SetActive(false);
        audioMenu.SetActive(true);
    }

    //Abre o menu do audio
    public void QuitButton()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        Time.timeScale = 1;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);

        //Dá load à cena do menu principal
        SceneManager.LoadScene("MainMenu");
    }

    //Volta de qualquer sub-menu para o menu pausa
    public void BackToPauseMenu()
    {
        //Toca o sound effect de click
        buttonClickSoundFX.Play();

        //Desliga os sub-menus
        audioMenu.SetActive(false);

        //Liga o menu pausa
        pauseMenu.SetActive(true);
    }
}
