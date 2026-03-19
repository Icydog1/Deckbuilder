using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FigureStats : MonoBehaviour
{
    [SerializeField]
    protected GameObject healthTextObject, condtionsTextObject;
    protected TextMeshProUGUI healthText, condtionsText;
    private bool noCondions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        condtionsText = condtionsTextObject.GetComponent<TextMeshProUGUI>();

        SetHealthAndBlock(100, 0);
        SetCondtions(new string[0]);
        List<string> testString = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealthAndBlock(int health, int block)
    {
        healthText.SetText("Health: " + health + " Block: " + block);
    }
    public void SetCondtions(string[] condtions)
    {
        if (condtions.Length == 0)
        {
            noCondions = true;
            condtionsText.SetText("");
        }
        else
        {
            condtionsText.SetText("Condtions " + condtions);
        }

    }
}
