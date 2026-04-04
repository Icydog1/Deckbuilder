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
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DisplayText(int variable)
    {
        textBox.enabled = true;
        displayedString = addionalText;
        displayedString += variable;
        textBox.SetText(displayedString);
    }
    public void DisplayString(string variable)
    {
        textBox.enabled = true;
        displayedString = addionalText;
        displayedString += variable;
        textBox.SetText(displayedString);
    }
    public void Disable()
    {
        textBox.enabled = false;
    }
}
