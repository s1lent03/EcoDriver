using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using UnityEngine.UI;

public class AudioMenuManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider AmbienceSlider;
    [SerializeField] Slider SoundFxSlider;

    [Header("Mixers")]
    [SerializeField] AudioMixer MusicMixer;
    [SerializeField] AudioMixer AmbienceMixer;
    [SerializeField] AudioMixer SoundFxMixer;

    public void OnMusicSliderValueChanged()
    {
        MusicMixer.SetFloat("MusicVolume", MusicSlider.value);
    }

    public void OnAmbienceSliderValueChanged()
    {
        AmbienceMixer.SetFloat("AmbienceVolume", AmbienceSlider.value);
    }

    public void OnSoundFxSliderValueChanged()
    {
        SoundFxMixer.SetFloat("SoundFxVolume", SoundFxSlider.value);
    }

    public void UpdateSlidersBasedOnFile()
    {
        string path = PlayerPrefs.GetString("SettingsPath");
        string[] lines = File.ReadAllLines(path);

        for (int lineNumber = 2; lineNumber <= 4; lineNumber++)
        {
            int startOfWord = lines[lineNumber].IndexOf("_");
            int CurrentQuality;

            switch (lineNumber)
            {
                case 2:

                    CurrentQuality = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    MusicSlider.value = CurrentQuality;
                    break;
                case 3:
                    CurrentQuality = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    SoundFxSlider.value = CurrentQuality;
                    break;
                case 4:
                    CurrentQuality = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    AmbienceSlider.value = CurrentQuality;
                    break;
            }
        }
    }

    public void UpdateMixersBasedOnFile()
    {
        string path = PlayerPrefs.GetString("SettingsPath");
        string[] lines = File.ReadAllLines(path);

        for (int lineNumber = 2; lineNumber <= 4; lineNumber++)
        {
            int startOfWord = lines[lineNumber].IndexOf("_");
            int CurrentQuality;

            switch (lineNumber)
            {
                case 2:
                    
                    CurrentQuality = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    MusicMixer.SetFloat("MusicVolume", CurrentQuality);
                    break;
                case 3:
                    CurrentQuality = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    SoundFxMixer.SetFloat("SoundFxVolume", CurrentQuality);
                    break;
                case 4:
                    CurrentQuality = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    AmbienceMixer.SetFloat("AmbienceVolume", CurrentQuality);
                    break;
            }
        }
    }

    public void SaveMixerValues()
    {
        string path = PlayerPrefs.GetString("SettingsPath");
        string[] lines = File.ReadAllLines(path);

        for (int lineNumber = 2; lineNumber <= 4; lineNumber++)
        {
            switch (lineNumber)
            {
                case 2:
                    lines[lineNumber] = "Music Value: _" + MusicSlider.value;
                    break;
                case 3:
                    lines[lineNumber] = "SoundFX Value: _" + SoundFxSlider.value;
                    break;
                case 4:
                    lines[lineNumber] = "Ambience Value: _" + AmbienceSlider.value;
                    break;
            }
        }

        File.WriteAllLines(path, lines);
    }
}
