using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FigureStats : MonoBehaviour
{
    protected GameObject healthTextObject, conditionsTextObject, planTextObject;
    protected TextMeshProUGUI healthText, conditionsText, planText;
    protected Figure figure;
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
        if (isPlayerUI)
        {
            figure = GameObject.Find("Player").GetComponent<PlayerControler>();
        }
        else
        {
            figure = transform.parent.GetComponent<Figure>();

        }

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

        for (int i = conditions.Count; i > 0; i--)
        {
            if (conditions[i - 1].IsVisible == false)
            {
                conditions.RemoveAt(i-1);
            }
        }
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
                currentConditionText = condition.Name;
                if (condition.Name == "nextTurns")
                {
                    currentConditionText = "Start of turn ";
                }
                if (condition.Plan != null)
                {
                    bool oldIsPlanning = figure.IsPlanning;
                    figure.IsPlanning = true;
                    List<string> conditionPlanDescription = new List<string>();
                    figure.PlanDescription = conditionPlanDescription;
                    
                    foreach (System.Action action in condition.Plan)
                    {
                        action();
                        if (condition.Plan[0] != action)
                        {
                            conditionPlanDescription[conditionPlanDescription.Count - 2] = conditionPlanDescription[conditionPlanDescription.Count - 2] + " and " + conditionPlanDescription[conditionPlanDescription.Count - 1];
                            conditionPlanDescription.RemoveAt(conditionPlanDescription.Count - 1);
                        }
                    }
                    
                    //condition.Plan();
                    foreach (string actionString in conditionPlanDescription)
                    {
                        currentConditionText += actionString;
                    }
                    //Debug.Log(conditionPlanDescription + " conditionPlanDescription first");
                    //Debug.Log(conditionPlanDescription + " conditionPlanDescription");
                    //Debug.Log(currentConditionText + " currentConditionText");
                    figure.IsPlanning = oldIsPlanning;
                    //currentConditionText += conditionPlanDescription;
                }
                if (condition.Value != 0)
                {
                    currentConditionText += " " + condition.Value + "<sprite name=PlacehoderConditonPower>";
                }
                if (condition.Duration != -1)
                {
                    currentConditionText += " " + condition.Duration + "<sprite name=PlacehoderConditonDuration>";
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
