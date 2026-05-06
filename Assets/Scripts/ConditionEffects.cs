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
        if (effectedFigure.UnmodifiedAction)
        {
            return initalAttack;
        }
        float modifiedAttack = initalAttack;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "Strength")
            {
                modifiedAttack += condition.Value;
            }
        }
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "NaturalScaling")
            {
                modifiedAttack *= (1 + 0.0025f * (float)condition.Value);
            }
        }
        int finalAttack = Mathf.FloorToInt(modifiedAttack);
        finalAttack = Mathf.Clamp(finalAttack, 0, 9999999);
        return finalAttack;
    }

    public int ModifyBlock(Figure effectedFigure, int initalBlock)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            return initalBlock;
        }
        float modifiedBlock = initalBlock;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "Dexterity")
            {
                modifiedBlock += condition.Value;
            }
        }
        int finalBlock = Mathf.FloorToInt(modifiedBlock);
        finalBlock = Mathf.Clamp(finalBlock, 0, 9999999);

        return finalBlock;
    }

    public int ModifyMove(Figure effectedFigure, int initalMove)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            return initalMove;
        }
        float modifiedMove = initalMove;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "Speed")
            {
                modifiedMove += condition.Value;
            }
        }
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "NaturalScaling")
            {
                modifiedMove *= (1 + 0.0025f * (float)condition.Value);
            }
            if (condition.Name == "DistanceSpeedBoost")
            {
                modifiedMove *= (1 + 0.03f * (float)condition.Value);
            }
        }
        int finalMove = Mathf.FloorToInt(modifiedMove);
        finalMove = Mathf.Clamp(finalMove, 0, 9999999);

        return finalMove;
    }

    public int ModifyAbility(Figure effectedFigure, int initalAbility)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            return initalAbility;
        }
        float modifiedAbility = initalAbility;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "Finesse")
            {
                modifiedAbility += condition.Value;
            }
        }
        int finalAbility = Mathf.FloorToInt(modifiedAbility);
        finalAbility = Mathf.Clamp(finalAbility, 0, 9999999);

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
            if (condition.Name == "DistanceJump")
            {
                modifiedJump = true;
            }
        }
        bool finalJump = modifiedJump;
        return finalJump;
    }

    public void StartOfTurnConditons(Figure effectedFigure)
    {
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.Name == "Poison")
            {
                effectedFigure.LoseHealth(condition.Value);
            }
            if (condition.Name == "NextTurns")
            {
                foreach (System.Action action in condition.Plan)
                {
                    action();
                }
                //condition.Plan();


                //GetComponent<NextTurns>.Action();
                //effectedFigure.Action();
            }

        }
    }
}
