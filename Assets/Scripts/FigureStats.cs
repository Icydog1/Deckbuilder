using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class FigureStats : MonoBehaviour
{
    protected GameObject healthTextObject, conditionsTextObject, planTextObject;
    protected TextMeshProUGUI healthText, conditionsText, planText;
    protected Figure figure;
    protected ActionManager actionManager;


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

        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();

        if (isPlayerUI)
        {
            figure = GameObject.Find("Player").GetComponent<PlayerControler>();
        }
        else
        {
            figure = transform.parent.GetComponent<Figure>();

        }
        //Debug.Log("stats awake ran");
        //SetHealthAndBlock(100, 0);
        //StartCoroutine(DisplayConditions(new List<Condition>()));
        //Plan(new List<string>());

    }

    public void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetHealthAndBlock(int health, int maxHealth, int block)
    {
        if (block > 0)
        {
            healthText.SetText("Health: " + health + "/" + maxHealth + " Block: " + block);
        }
        else
        {
            healthText.SetText("Health: " + health + "/" + maxHealth);
        }
    }

    public IEnumerator DisplayConditions(List<Condition> currentConditions)
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
                //Debug.Log("displaying condition: " + condition.ConditionName);
                if (condition.ActiveDescription != null)
                {
                    currentConditionText = condition.ActiveDescription;
                }
                else
                {
                    currentConditionText = condition.ConditionName;
                    if (condition.ConditionName == "NextTurns")
                    {
                        currentConditionText = "Start of turn ";
                    }
                    if (condition.Plan != null)
                    {

                        //List<ActionDescription> conditionPlanDescription = new List<ActionDescription>();
                        List<ActionDescription> conditionPlanDescription = condition.PlanDescription;
                        //foreach (Func<IEnumerator> action in condition.Plan)
                        //{
                        //    yield return StartCoroutine(actionManager.PreformAction(action(), conditionPlanDescription));
                        //    //if (condition.Plan[0] != action)
                        //    //{
                        //    //    conditionPlanDescription[conditionPlanDescription.Count - 2] = conditionPlanDescription[conditionPlanDescription.Count - 2] + " and " + conditionPlanDescription[conditionPlanDescription.Count - 1];
                        //    //    conditionPlanDescription.RemoveAt(conditionPlanDescription.Count - 1);
                        //    //}
                        //}
                        //currentConditionText += string.Join(" and ", conditionPlanDescription);

                        for (int i = 0; i < conditionPlanDescription.Count; i++)
                        {
                            if (i == conditionPlanDescription.Count - 1 && i > 0)
                            {
                                currentConditionText += " and ";
                            }
                            else if (i != 0)
                            {
                                currentConditionText += ",";
                            }
                            currentConditionText += conditionPlanDescription[i].GetDescription();

                        }
                        //condition.Plan();
                        //foreach (Action action in conditionPlanDescription)
                        //{
                        //    currentConditionText += action.GetDescription();
                        //}
                        //Debug.Log(conditionPlanDescription + " conditionPlanDescription first");
                        //Debug.Log(conditionPlanDescription + " conditionPlanDescription");
                        //Debug.Log(currentConditionText + " currentConditionText");
                        //currentConditionText += conditionPlanDescription;
                    }

                }
                if (condition.Value != Variables.gameNullValue)
                {
                    currentConditionText += " " + condition.Value + "<sprite name=ConditionPower>";
                }
                if (condition.Duration != Variables.gameInfinityValue)
                {
                    currentConditionText += " " + condition.Duration + "<sprite name=ConditionDuration>";
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
        yield break;
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

    public void Plan(List<ActionDescription> moves)
    {
        MovePlan();
        string movesDisplay = null;
        foreach (ActionDescription move in moves)
        {
            if (movesDisplay == null)
            {
                movesDisplay = move.GetDescription();
            }
            else
            {
                //Debug.Log("planed move");
                movesDisplay += ", " + move.GetDescription();
            }
        }
        //Debug.Log(movesDisplay);
        planText.text = "Plan: " + movesDisplay;
    }

    public void ChangePlan(string action, int newValue)
    {
        Regex regex = new Regex("([0-9]+)( " + action + ")");
        planText.text = regex.Replace(planText.text, newValue + "$2",1);
        //planText.text = Regex.Replace(planText.text, "(.+)", "$1");

    }
    public virtual void SetLevelAndXP(int Level, int potenialLevel, int XP, int XPThreshold)
    {
        //levelAndXPText.SetText("Level: " + Level + "(" + potenialLevel + "), XP: " + XP + "/" + XPThreshold);
    }
    public virtual void SetTurnCount(int turnCount)
    {
        //levelAndXPText.SetText("Level: " + Level + "(" + potenialLevel + "), XP: " + XP + "/" + XPThreshold);
    }
}
