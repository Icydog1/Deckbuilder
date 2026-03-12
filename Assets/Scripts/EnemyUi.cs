using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyUi : MonoBehaviour
{
    public GameObject healthTextObject, condtionsTextObject, planTextObject;
    private TextMeshProUGUI healthText, condtionsText, planText;

    //List<string> currentCondtions = new List<string>();
    private bool noCondions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        condtionsText = condtionsTextObject.GetComponent<TextMeshProUGUI>();
        planText = planTextObject.GetComponent<TextMeshProUGUI>();



        //SetHealth(100);
        SetCondtions(new string[0]);
        List<string> testString = new List<string>();
        //testString.Add("Move 2");
        //testString.Add("Move 3");
        //testString.Add("Attack 4");
        Plan(testString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int health)
    {
        healthText.SetText("Health: " + health);
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
    public void Plan(List<string> moves)
    {
        if (noCondions)
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0.25f , 0);
        }
        else
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0.0f, 0);
        }
        string movesDisplay = "";
        foreach (string move in moves)
        {
            if (movesDisplay == "")
            {
                movesDisplay = move;
            }
            else
            {
                movesDisplay += ", " + move;
            }
        }
        planText.SetText("Plan: " + movesDisplay);
    }
}
