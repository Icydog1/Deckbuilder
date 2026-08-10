using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ActionPreformer//, IEquatable<Ability>
{

    protected int cost;
    public int Cost { get { return cost; } }

    protected int abilityValue, timesPreformed;
    private int maxTimes;
    public int MaxTimes { get { return maxTimes; } set { maxTimes = value; } }

    private bool isUsed;
    //private List<Func<IEnumerator>> abilities = new List<Func<IEnumerator>>();
    //public List<Func<IEnumerator>> Abilities { get { return abilities; } }
    private List<Action> actions = new List<Action>();
    public List<Action> Actions { get { return actions; } }

    protected List<ActionDescription> description = new List<ActionDescription>();

    private AbilityUI abilityUI;
    public AbilityUI AbilityUI { get { return abilityUI; } set { abilityUI = value; } }

    protected PlayerControler playerControler;
    protected MouseManager mouseManager;
    protected AbilityManager abilityManager;
    protected ActionManager actionManager;

    public void Gained()
    {
        playerControler = RefrenceStorage.playerControler;
        mouseManager = RefrenceStorage.mouseManager;
        abilityManager = RefrenceStorage.abilityManager;
        actionManager = RefrenceStorage.actionManager;
        playerControler.PlayerTurnStartedFuntions += ResetAbilityCooldown;
    }
    public void ResetAbilityCooldown(PlayerControler playerControler)
    {
        if (isUsed)
        {
            isUsed = false;
            abilityUI.DisplayUsed(false);
        }
    }
    public Ability(int abilityCost, List<Action> preformedActions, int maxUses = Var.maxValue)
    {
        actions = preformedActions;
        cost = abilityCost;
        maxTimes = maxUses;
    }
    
    public IEnumerator UpdateDiscription(int abilitiesPointsSpent)
    {
        description.Clear();
        int potentialTimesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
        potentialTimesPreformed = Mathf.Min(potentialTimesPreformed, maxTimes);
        playerControler.VariableCardModifier = potentialTimesPreformed;
        playerControler.UnmodifiedAction = true;
        foreach (Action action in actions)
        {
            yield return abilityManager.StartCoroutine(action.PreformAction(this, description));

        }
        playerControler.UnmodifiedAction = false;

        abilityUI.DisplayText(description);
    }

    public IEnumerator PreformAbility(int abilitiesPointsSpent)
    {
        if (playerControler.CanPreformAbilities && !isUsed)
        {
            timesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
            timesPreformed = Mathf.Min(timesPreformed, maxTimes);
            if (timesPreformed >= 1)
            {
                isUsed = true;
                abilityUI.DisplayUsed(true);
                mouseManager.MouseOffObject(abilityUI.gameObject);
                playerControler.ActionsRemaining = new List<ActionDescription>(description);
                abilityManager.AbilityPower -= timesPreformed * cost;
                //abilityManager.SelectedPower = abilityManager.SelectedPower;
                yield return abilityManager.StartCoroutine(abilityManager.SetSelectedPower(abilityManager.SelectedPower));

                playerControler.VariableCardModifier = timesPreformed;
                playerControler.PreformingAbility = true;
                playerControler.ActiveActionPreformer = this;
                actionManager.ActionEnded = false;
                //playerControler.NextAction = false;
                foreach (Action action in actions)
                {
                    playerControler.UnmodifiedAction = true;
                    //yield return abilityManager.StartCoroutine(actionManager.PreformAction(action()));
                    yield return abilityManager.StartCoroutine(action.PreformAction(this, null));

                    playerControler.UnmodifiedAction = false;
                    //yield return new WaitUntil(() => playerControler.NextAction == true);
                    //playerControler.NextAction = false;
                }
                StopCommanding();
                //Debug.Log("done");
                playerControler.PreformingAbility = false;
            }
        }

    }


}