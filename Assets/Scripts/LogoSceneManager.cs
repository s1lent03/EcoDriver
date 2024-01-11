using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoSceneManager : MonoBehaviour
{
    public float duration;
    private float num = 1;

    private void Start()
    {
        PlayerPrefs.SetString("isFullscreen", "true");
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    void Update()
    {
        num = num + (1 * Time.deltaTime);

        if (num >= duration)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
