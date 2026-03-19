using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VariableDisplayer : MonoBehaviour
{
    private TextMeshProUGUI textBox;

    private string displayedString;
    [SerializeField]
    private string addionalText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        textBox = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DisplayText(float variable)
    {
        displayedString = addionalText;
        displayedString += variable;
        textBox.SetText(displayedString);
    }
}
