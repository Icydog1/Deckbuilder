using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionEffects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int ModifyAttack(Figure effectedFigure, int initalAttack)
    {
        //Debug.Log("Modifing Attack");
        if (effectedFigure.UnmodifiedAction)
        {
            initalAttack = Mathf.Clamp(initalAttack, 0, Variables.gameMaxValue);
            return initalAttack;
        }
        float modifiedAttack = initalAttack;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "NaturalScaling")
            {
                modifiedAttack *= (1 + 0.0025f * (float)condition.Value);
            }
        }
        foreach (Condition condition in conditions)
        {
            //Debug.Log("checked " + condition.ConditionName);
            if (condition.ConditionName == "Strength" || condition.ConditionName == "Vigor")
            {
                //Debug.Log("Added Attack");
                modifiedAttack += condition.Value;
            }
        }

        int finalAttack = Mathf.FloorToInt(modifiedAttack);
        finalAttack = Mathf.Clamp(finalAttack, 0, Variables.gameMaxValue);
        return finalAttack;
    }

    public int ModifyBlock(Figure effectedFigure, int initalBlock)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initalBlock = Mathf.Clamp(initalBlock, 0, Variables.gameMaxValue);
            return initalBlock;
        }
        float modifiedBlock = initalBlock;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Dexterity")
            {
                modifiedBlock += condition.Value;
            }
        }
        int finalBlock = Mathf.FloorToInt(modifiedBlock);
        finalBlock = Mathf.Clamp(finalBlock, 0, Variables.gameMaxValue);

        return finalBlock;
    }

    public int ModifyMove(Figure effectedFigure, int initalMove)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initalMove = Mathf.Clamp(initalMove, 0, Variables.gameMaxValue);
            return initalMove;
        }
        float modifiedMove = initalMove;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "NaturalScaling")
            {
                modifiedMove *= (1 + 0.0025f * (float)condition.Value);
            }
            if (condition.ConditionName == "DistanceSpeedBoost")
            {
                modifiedMove *= (1 + 0.03f * (float)condition.Value);
            }
        }
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Speed")
            {
                modifiedMove += condition.Value;
            }
        }

        int finalMove = Mathf.FloorToInt(modifiedMove);
        finalMove = Mathf.Clamp(finalMove, 0, Variables.gameMaxValue);

        return finalMove;
    }

    public int ModifyAbility(Figure effectedFigure, int initalAbility)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initalAbility = Mathf.Clamp(initalAbility, 0, Variables.gameMaxValue);
            return initalAbility;
        }
        float modifiedAbility = initalAbility;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Finesse")
            {
                modifiedAbility += condition.Value;
            }
        }
        int finalAbility = Mathf.FloorToInt(modifiedAbility);
        finalAbility = Mathf.Clamp(finalAbility, 0, Variables.gameMaxValue);

        return finalAbility;
    }

    public bool ModifyJump(Figure effectedFigure, bool initalJump)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            return initalJump;
        }
        bool modifiedJump = initalJump;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "DistanceJump")
            {
                //Debug.Log("ModifyJump");
                modifiedJump = true;
            }
        }
        bool finalJump = modifiedJump;
        return finalJump;
    }

    public IEnumerator StartOfTurnConditons(Figure effectedFigure)
    {
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Poison")
            {
                effectedFigure.LoseHealth(condition.Value);
            }
            if (condition.ConditionName == "NextTurns")
            {
                foreach (Func<IEnumerator> action in condition.Plan)
                {
                    yield return StartCoroutine(action());
                }
                //condition.Plan();


                //GetComponent<NextTurns>.Action();
                //effectedFigure.Action();
            }

        }
    }
}
