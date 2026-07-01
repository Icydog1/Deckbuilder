using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class ConditionEffects : MonoBehaviour
{
    ActionManager actionManager;

    PlayerControler effectedPlayerControler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        actionManager = RefrenceStorage.actionManager;
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
                modifiedAttack *= (1 + Variables.naturalScalingIncrease * (float)condition.Value);
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
                modifiedMove *= (1 + Variables.naturalScalingIncrease * (float)condition.Value);
            }
            if (condition.ConditionName == "DistanceSpeedBoost")
            {
                modifiedMove *= (1 + 0.03f * (float)condition.Value);
            }
        }
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Speed" || condition.ConditionName == "Burst")
            {
                modifiedMove += condition.Value;
            }
        }

        int finalMove = Mathf.FloorToInt(modifiedMove);
        finalMove = Mathf.Clamp(finalMove, 0, Variables.gameMaxValue);

        return finalMove;
    }

    public int ModifySkill(Figure effectedFigure, int initalSkill)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initalSkill = Mathf.Clamp(initalSkill, 0, Variables.gameMaxValue);
            return initalSkill;
        }
        float modifiedSkill = initalSkill;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Finesse")
            {
                modifiedSkill += condition.Value;
            }
        }
        int finalSkill = Mathf.FloorToInt(modifiedSkill);
        finalSkill = Mathf.Clamp(finalSkill, 0, Variables.gameMaxValue);

        return finalSkill;
    }
    public int ModifyRange(Figure effectedFigure, int initalRange)
    {
        if (initalRange == Variables.gameInfinityValue)
        {
            return initalRange;
        }
        if (effectedFigure.UnmodifiedAction)
        {
            initalRange = Mathf.Clamp(initalRange, 1, Variables.gameMaxValue);
            return initalRange;
        }
        float modifiedRange = initalRange;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Accuracy")
            {
                if (modifiedRange > 1)
                {
                    modifiedRange += condition.Value;
                }
            }
        }
        int finalRange = Mathf.FloorToInt(modifiedRange);
        finalRange = Mathf.Clamp(finalRange, 1, Variables.gameMaxValue);
        return finalRange;
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
    public int ModifyMaxHealth(Figure effectedFigure, int initialMaxHealth)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initialMaxHealth = Mathf.Clamp(initialMaxHealth, 0, Variables.gameMaxValue);
            return initialMaxHealth;
        }
        float modifiedMaxHealth = initialMaxHealth;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "NaturalScaling")
            {
                modifiedMaxHealth *= (1 + Variables.naturalScalingIncrease * (float)condition.Value);
            }
        }
        int finalMaxHealth = Mathf.FloorToInt(modifiedMaxHealth);
        finalMaxHealth = Mathf.Clamp(finalMaxHealth, 0, Variables.gameMaxValue);

        return finalMaxHealth;
    }
    public IEnumerator StartOfTurnConditions(Figure effectedFigure)
    {
        if (effectedFigure is PlayerControler effectedPlayer)
        {
            // Access SubClassA specific methods or fields here
            effectedPlayerControler = effectedPlayer;
        }
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "Poison")
            {
                yield return RefrenceStorage.gameManager.StartCoroutine(effectedFigure.LoseHealth(condition.Value));
            }
            if (condition.ConditionName == "NextTurns")
            {
                effectedPlayerControler.SpecialPreformingAction = true;
                effectedPlayerControler.ActionsRemaining = new List<ActionDescription>(condition.PlanDescription);
                foreach (Func<IEnumerator> action in condition.Plan)
                {
                    yield return StartCoroutine(actionManager.PreformAction(action()));
                }
                effectedPlayerControler.SpecialPreformingAction = false;
            }
            if (condition.ConditionName == "StartOfTurnBlock")
            {
                effectedFigure.UnmodifiedAction = true;
                yield return StartCoroutine(actionManager.PreformAction(effectedFigure.Block(condition.Value)));
                effectedFigure.UnmodifiedAction = false;
            }
            if (condition.ConditionName == "NextTurnTopEnergy")
            {
                yield return StartCoroutine(actionManager.PreformAction(effectedPlayerControler.GainTopEnergy(condition.Value)));
            }
            if (condition.ConditionName == "NextTurnBottomEnergy")
            {
                yield return StartCoroutine(actionManager.PreformAction(effectedPlayerControler.GainBottomEnergy(condition.Value)));
            }
            //if (condition.ConditionName == "Next Turn Block")
            //{
            //    yield return StartCoroutine(effectedFigure.Block(condition.Value));
            //}
        }
    }
}
