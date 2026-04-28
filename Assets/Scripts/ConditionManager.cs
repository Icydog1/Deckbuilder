using System.Collections.Generic;
using UnityEngine;

public class ConditionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GainCondition(Figure effectedFigure, Condition condition)
    {
        //conditions.Add(condition);
    }

    public int ModifyAttack(Figure effectedFigure, int initalAttack)
    {
        float modifiedAttack = initalAttack;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "strength")
            {
                modifiedAttack += condition.Value;
            }
        }
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "naturalScaling")
            {
                modifiedAttack *= (1 + 0.00001f * (float)condition.Value);
            }
        }
        int finalAttack = Mathf.FloorToInt(modifiedAttack);
        return finalAttack;
    }

    public int ModifyBlock(Figure effectedFigure, int initalBlock)
    {
        float modifiedBlock = initalBlock;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "dexterity")
            {
                modifiedBlock += condition.Value;
            }
        }
        int finalBlock = Mathf.FloorToInt(modifiedBlock);
        return finalBlock;
    }

    public int ModifyMove(Figure effectedFigure, int initalMove)
    {
        float modifiedMove = initalMove;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "speed")
            {
                modifiedMove += condition.Value;
            }
        }
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "naturalScaling")
            {
                modifiedMove *= (1 + 0.00001f * (float)condition.Value);
            }
        }
        int finalMove = Mathf.FloorToInt(modifiedMove);
        return finalMove;
    }

    public int ModifyAbility(Figure effectedFigure, int initalAbility)
    {
        float modifiedAbility = initalAbility;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "finesse")
            {
                modifiedAbility += condition.Value;
            }
        }
        int finalAbility = Mathf.FloorToInt(modifiedAbility);
        return finalAbility;
    }
}
