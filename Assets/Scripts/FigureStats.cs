using System.Collections.Generic;
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

        SetHealthAndBlock(100, 0);
        DisplayConditions(new List<Condition>());
        Plan(new List<string>());

    }

    public void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetHealthAndBlock(int health, int block)
    {
        if (block > 0)
        {
            healthText.SetText("Health: " + health + " Block: " + block);
        }
        else
        {
            healthText.SetText("Health: " + health);
        }
    }

    public void DisplayConditions(List<Condition> currentConditions)
    {
        List<Condition> conditions = new List<Condition>(currentConditions);
        //Debug.Log("first condition: " + conditions[0].Name);
        //Debug.Log("condition count: " + conditions.Count);
        //Debug.Log("Displaed Conditions 1 time");

        for (int i = conditions.Count; i > 0; i--)
        {
            //Condition checkedCondition = conditions[i-1];
            //if (conditions.Count == 0)
            //{
            //    Debug.Log("no conditions");
            //}
            ////Debug.Log("ran 1 time");
            //Debug.Log("current condition: " + checkedCondition.Name);
            //Debug.Log("current condition visibility: " + checkedCondition.IsVisible);
            if (conditions[i - 1].IsVisible == false)
            {
                //Debug.Log("condition count before: " + conditions.Count);

                //Debug.Log("first condition before: " + conditions[0].Name);
                ////Debug.Log("didnt display on " + i);
                //Debug.Log("didnt display " + checkedCondition.Name);
                conditions.RemoveAt(i-1);
                //if (conditions.Count != 0)
                //{
                //    Debug.Log("first condition after: " + conditions[0].Name);
                //    Debug.Log("condition count after: " + conditions.Count);
                //}
            }
        }
        //if (conditions.Count != 0)
        //{
        //    Debug.Log("first condition: " + conditions[0].Name);
        //}
        //if (conditions.Count == 0)
        //{
        //    Debug.Log("no conditions");
        //}
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
                //Debug.Log("displaying condition: " + condition.Name);

                currentConditionText = condition.Name + " " + condition.Value;
                if (condition.Duration != -1)
                {
                    currentConditionText += " " + condition.Duration;
                }
                individualConditionText.Add(currentConditionText);
            }
            string separator = ", ";

            string conditionText = "Conditions: " + string.Join(separator, individualConditionText);
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
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0.0f, 0);
        }
        else
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, -0.25f, 0);
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
        //Debug.Log(movesDisplay);
        planText.text = "Plan: " + movesDisplay;
    }
}
