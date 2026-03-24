using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FigureStats : MonoBehaviour
{
    protected GameObject healthTextObject, conditionsTextObject;
    protected TextMeshProUGUI healthText, conditionsText;
    protected bool noConditions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        healthTextObject = transform.Find("HealthText").gameObject;
        conditionsTextObject = transform.Find("ConditionsText").gameObject;
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        conditionsText = conditionsTextObject.GetComponent<TextMeshProUGUI>();

        SetHealthAndBlock(100, 0);
        SetConditions(new string[0]);
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

    public void SetConditions(string[] conditions)
    {
        if (conditions.Length == 0)
        {
            noConditions = true;
            conditionsText.SetText("");
        }
        else
        {
            conditionsText.SetText("Conditions " + conditions);
        }

    }
}
