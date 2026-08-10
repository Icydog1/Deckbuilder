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

    public int ModifyAttack(Figure effectedFigure, int initalAttack)
    {
        //Debug.Log("Modifing Attack");
        if (effectedFigure.UnmodifiedAction)
        {
            initalAttack = Global.Clamp(initalAttack,0);
            return initalAttack;
        }
        float modifiedAttack = initalAttack;
        int addedAttack = 0;
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "NaturalScaling")
            {
                modifiedAttack *= OverallStatistics.difficulty;
            }
            else if (condition.ConditionName == "Strength" || condition.ConditionName == "Vigor")
            {
                addedAttack += condition.Value;
            }

        }
        modifiedAttack += addedAttack;
        //foreach (Condition condition in conditions)
        //{
        //    //Debug.Log("checked " + condition.ConditionName);
        //    if (condition.ConditionName == "Strength" || condition.ConditionName == "Vigor")
        //    {
        //        //Debug.Log("Added Attack");
        //        modifiedAttack += condition.Value;
        //    }
        //}

        int finalAttack = Mathf.FloorToInt(modifiedAttack);
        finalAttack = Global.Clamp(finalAttack, 0);
        return finalAttack;
    }

    public int ModifyBlock(Figure effectedFigure, int initalBlock)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initalBlock = Global.Clamp(initalBlock, 0);
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
        finalBlock = Global.Clamp(finalBlock, 0);

        return finalBlock;
    }

    public int ModifyMove(Figure effectedFigure, int initalMove)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initalMove = Global.Clamp(initalMove, 0);
            return initalMove;
        }
        float modifiedMove = initalMove;
        List<Condition> conditions = effectedFigure.Conditions;
        int addedMove = 0;
        float multipliedMove = 1;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "NaturalScaling")
            {
                modifiedMove *= OverallStatistics.difficulty;
            }
            else if (condition.ConditionName == "DistanceSpeedBoost")
            {
                multipliedMove *= (1 + 0.03f * (float)condition.Value);
            }
            else if(condition.ConditionName == "Speed" || condition.ConditionName == "Burst")
            {
                addedMove += condition.Value;
            }
        }
        modifiedMove += addedMove;
        modifiedMove *= multipliedMove;
        //foreach (Condition condition in conditions)
        //{
        //    if (condition.ConditionName == "Speed" || condition.ConditionName == "Burst")
        //    {
        //        modifiedMove += condition.Value;
        //    }
        //}

        int finalMove = Mathf.FloorToInt(modifiedMove);
        finalMove = Global.Clamp(finalMove, 0);

        return finalMove;
    }

    public int ModifySkill(Figure effectedFigure, int initalSkill)
    {
        if (effectedFigure.UnmodifiedAction)
        {
            initalSkill = Global.Clamp(initalSkill, 0);
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
        finalSkill = Global.Clamp(finalSkill, 0);

        return finalSkill;
    }
    public int ModifyRange(Figure effectedFigure, int initalRange)
    {
        if (initalRange == Var.infinityValue)
        {
            return initalRange;
        }
        if (effectedFigure.UnmodifiedAction)
        {
            initalRange = Global.Clamp(initalRange, 1);
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
        finalRange = Global.Clamp(finalRange, 1);
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
            initialMaxHealth = Global.Clamp(initialMaxHealth, 0);
            return initialMaxHealth;
        }
        float modifiedMaxHealth = initialMaxHealth;
        if (effectedFigure.IsPlayerSummon)
        {
            if (effectedFigure.Summoner is PlayerControler effectedPlayer)
            {
                // Access SubClassA specific methods or fields here
                effectedPlayerControler = effectedPlayer;
            }
            modifiedMaxHealth += effectedPlayerControler.EnchantedBoltsCount * Var.enchantedBoltsMaxHealth;
        }
        List<Condition> conditions = effectedFigure.Conditions;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == "NaturalScaling")
            {
                modifiedMaxHealth *= OverallStatistics.difficulty;
            }
        }
        int finalMaxHealth = Mathf.FloorToInt(modifiedMaxHealth);
        finalMaxHealth = Global.Clamp(finalMaxHealth, 0);

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
        for(int i = 0; i < conditions.Count; i++)
        //foreach (Condition condition in conditions)
        {
            Condition condition = conditions[i];
            if (condition.ConditionName == "Poison")
            {
                yield return RefrenceStorage.gameManager.StartCoroutine(effectedFigure.LoseHealth(condition.Value));
                condition.Value--;
                if (condition.Value == 0)
                {
                    i--;
                    yield return StartCoroutine(effectedFigure.RemoveCondition("Poison"));
                }
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
                effectedPlayerControler.StartingTopEnergy += condition.Value;

                //yield return StartCoroutine(actionManager.PreformAction(effectedPlayerControler.GainTopEnergy(condition.Value)));
            }
            if (condition.ConditionName == "NextTurnBottomEnergy")
            {
                effectedPlayerControler.StartingBottomEnergy += condition.Value;
                //yield return StartCoroutine(actionManager.PreformAction(effectedPlayerControler.GainBottomEnergy(condition.Value)));
            }
            if (condition.ConditionName == "NextTurnCards")
            {
                effectedPlayerControler.StartingCards += condition.Value;
                //yield return StartCoroutine(actionManager.PreformAction(effectedPlayerControler.GainBottomEnergy(condition.Value)));
            }
            //if (condition.ConditionName == "Next Turn Block")
            //{
            //    yield return StartCoroutine(effectedFigure.Block(condition.Value));
            //}
        }
    }
}
