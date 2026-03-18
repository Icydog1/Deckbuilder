using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardEffectText : MonoBehaviour
{
    private TextMeshProUGUI textBox;
    private string displayedString;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        textBox = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayText(List<string> displayedText)
    {
        displayedString = "";
        foreach (string text in displayedText)
        {
            displayedString += text;
            displayedString += "\n";
        }
        textBox.SetText(displayedString);

    }
}
