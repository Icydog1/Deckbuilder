using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability//, IEquatable<Ability>
{

    protected int cost;
    public int Cost { get { return cost; } }

    protected int abilityValue, timesPreformed;
    private int maxTimes;
    public int MaxTimes { get { return maxTimes; } set { maxTimes = value; } }

    private bool isUsed;
    private List<Func<IEnumerator>> abilities = new List<Func<IEnumerator>>();
    public List<Func<IEnumerator>> Abilities { get { return abilities; } }

    protected List<ActionDescription> description = new List<ActionDescription>();

    private AbilityUI abilityUI;
    public AbilityUI AbilityUI { get { return abilityUI; } set { abilityUI = value; } }

    protected PlayerControler playerControler;
    protected MouseManager mouseManager;
    protected AbilityManager abilityManager;
    protected ActionManager actionManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Gained()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();

        //abilityUI = transform.Find("AbilityUI").GetComponent<AbilityUI>();
        playerControler.PlayerTurnStartedFuntions += ResetAbilityCooldown;
    }
    //public bool Equals(Ability other)
    //{
    //    if (other.Abilities == this.abilities && other.Cost == this.Cost)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
    public void ResetAbilityCooldown(PlayerControler playerControler)
    {
        if (isUsed)
        {
            isUsed = false;
            abilityUI.DisplayUsed(false);
        }
    }

    public Ability(int abilityCost, List<Func<IEnumerator>> preformedAbilities, int maxUses = Variables.gameMaxValue)
    {
        abilities = preformedAbilities;
        cost = abilityCost;
        maxTimes = maxUses;
    }
    
    public IEnumerator UpdateDiscription(int abilitiesPointsSpent)
    {

        description.Clear();
        //playerControler.IsPlanning = true;
        //actionManager.PlanToList = description;
        int potentialTimesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
        potentialTimesPreformed = Mathf.Min(potentialTimesPreformed, maxTimes);
        playerControler.VariableCardModifier = potentialTimesPreformed;
        playerControler.UnmodifiedAction = true;
        foreach (Func<IEnumerator> action in abilities)
        {
            //if (!(action != null))
            //{
            //    Debug.Log("action null");
            //}
            //if (description == null)
            //{
            //    Debug.Log("description null");
            //}
            //IEnumerator test = actionManager.PreformAction(action(), description);
            //Debug.Log(actionManager);
            yield return abilityManager.StartCoroutine(actionManager.PreformAction(action(), description));

        }
        playerControler.UnmodifiedAction = false;

        abilityUI.DisplayText(description);
        //playerControler.IsPlanning = false;
        //yield return null;
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
                playerControler.ActionEnded = false;
                //playerControler.NextAction = false;
                foreach (Func<IEnumerator> action in abilities)
                {
                    playerControler.UnmodifiedAction = true;
                    yield return abilityManager.StartCoroutine(actionManager.PreformAction(action()));
                    playerControler.UnmodifiedAction = false;
                    //yield return new WaitUntil(() => playerControler.NextAction == true);
                    //playerControler.NextAction = false;
                }

                //Debug.Log("done");
                playerControler.PreformingAbility = false;
            }
        }

    }


}

/*
public class LockpickAbility : Ability
{
    public override void Awake()
    {
        base.Awake();
        cost = 1;
        baseValue = 1;
    }
    public void PreformAbility(int abilitiesPointsSpent)
    {
        timesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
        abilityValue = timesPreformed * baseValue;
        playerControler.Lockpick(abilityValue);
    }
}

*/