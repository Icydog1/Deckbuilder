using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class FigureStats : MonoBehaviour
{
    protected GameObject healthTextObject, conditionsTextObject, planTextObject;
    protected TextMeshProUGUI healthText, conditionsText, planText;
    protected bool noConditions;
    protected bool isPlayerUI = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        healthTextObject = transform.Find("HealthText").gameObject;
        conditionsTextObject = transform.Find("ConditionsText").gameObject;
        planTextObject = transform.Find("PlanText").gameObject;
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        conditionsText = conditionsTextObject.GetComponent<TextMeshProUGUI>();
        planText = planTextObject.GetComponent<TextMeshProUGUI>();


    }

    public void Start()
    {
        SetHealthAndBlock(100, 0);
        DisplayConditions(new List<Condition>());
        Plan(new List<string>());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetHealthAndBlock(int health, int block)
    {
        healthText.SetText("Health: " + health + " Block: " + block);
    }

    public void DisplayConditions(List<Condition> conditions)
    {
        if (conditions.Count == 0)
        {
            noConditions = true;
            conditionsText.SetText("");
        }
        else
        {
            noConditions = false;
            List<string> individualConditionText = new List<string>();
            foreach (Condition condition in conditions)
            {
                string currentConditionText = "";

                currentConditionText = condition.Name + " " + condition.Value;
                if (condition.Duration != -1)
                {
                    currentConditionText += " " + condition.Duration;
                }
                individualConditionText.Add(currentConditionText);
            }
            string separator = ", ";

            string conditionText = "Conditions: " + string.Join(separator, individualConditionText); ;
            conditionsText.SetText(conditionText);

            //Debug.Log(conditionText);
            // Join the elements with the separator
        }
        //Debug.Log(conditions.Count);
        MovePlan();
    }
    public virtual void MovePlan()
    {
        if (noConditions)
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0.25f, 0);
        }
        else
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0.0f, 0);
        }
    }

    public void Plan(List<string> moves)
    {
        MovePlan();
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
