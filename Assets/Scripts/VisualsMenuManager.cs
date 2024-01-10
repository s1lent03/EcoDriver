using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VisualsMenuManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject HighSelected;
    [SerializeField] GameObject MediumSelected;
    [SerializeField] GameObject LowSelected;

    public void ChangeQualityToHighButton()
    {
        QualitySettings.SetQualityLevel(0, true);
        SwitchSelectedObject(HighSelected, MediumSelected, LowSelected);
        OverwriteQualityInFile("High");
    }

    public void ChangeQualityToMediumButton()
    {
        QualitySettings.SetQualityLevel(1, true);
        SwitchSelectedObject(MediumSelected, HighSelected, LowSelected);
        OverwriteQualityInFile("Medium");
    }

    public void ChangeQualityToLowButton()
    {
        QualitySettings.SetQualityLevel(2, true);
        SwitchSelectedObject(LowSelected, HighSelected, MediumSelected);
        OverwriteQualityInFile("Low");
    }

    public void SelectButtonBasedOnQuality(string Quality)
    {
        switch (Quality){
            case "High":
                SwitchSelectedObject(HighSelected, MediumSelected, LowSelected);
                break;

            case "Medium":
                SwitchSelectedObject(MediumSelected, HighSelected, LowSelected);
                break;

            case "Low":
                SwitchSelectedObject(LowSelected, HighSelected, MediumSelected);
                break;
        }
    }

    void SwitchSelectedObject(GameObject selectedObject, GameObject notSelectedObject1, GameObject notSelectedObject2)
    {
        //Ativar objeto para mostrar o selecionado e desativar os outros
        selectedObject.SetActive(true);
        notSelectedObject1.SetActive(false);
        notSelectedObject2.SetActive(false);
    }

    //Overwrite à setting de qualidade
    void OverwriteQualityInFile(string newQualityText)
    {
        string path = PlayerPrefs.GetString("SettingsPath");
        int lineNumber = 0;
        string[] lines = File.ReadAllLines(path);
        string OverwriteText = "Selected visual quality: _" + newQualityText;

        if (lineNumber >= 0 && lineNumber < lines.Length)
        {
            lines[lineNumber] = OverwriteText;

            File.WriteAllLines(path, lines);
        }
    }
}
