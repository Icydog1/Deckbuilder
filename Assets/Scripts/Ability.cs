using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour//, IEquatable<Ability>
{

    protected int cost;
    public int Cost { get { return cost; } }

    protected int abilityValue, timesPreformed;
    private int maxTimes;
    private bool isUsed;
    private List<IEnumerator> abilities = new List<IEnumerator>();
    public List<IEnumerator> Abilities { get { return abilities; } }

    protected List<string> description = new List<string>();

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
        PlayerControler.PlayerTurnStarted += ResetAbilityCooldown;
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
        isUsed = false;
        abilityUI.DisplayUsed(false);
    }

    public Ability(int abilityCost, List<IEnumerator> preformedAbilities)
    {
        abilities = preformedAbilities;
        cost = abilityCost;
    }
    
    public void UpdateDiscription(int abilitiesPointsSpent)
    {

        description.Clear();
        playerControler.IsPlanning = true;
        playerControler.PlanDescription = description;
        int potentialTimesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
        playerControler.VariableCardModifier = potentialTimesPreformed;
        playerControler.UnmodifiedAction = true;
        foreach (IEnumerator action in abilities)
        {
            StartCoroutine(actionManager.PreformAction(action));
        }
        playerControler.UnmodifiedAction = false;

        abilityUI.DisplayText(description);
        playerControler.IsPlanning = false;
    }

    public IEnumerator PreformAbility(int abilitiesPointsSpent)
    {
        if (playerControler.CanPreformAbilities && !isUsed)
        {
            timesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);

            if (timesPreformed >= 1)
            {
                isUsed = true;
                abilityUI.DisplayUsed(true);
                mouseManager.MouseOffObject(abilityUI.gameObject);
                playerControler.ActionsRemaining = new List<string>(description);
                abilityManager.AbilityPower -= timesPreformed * cost;
                abilityManager.SelectedPower = abilityManager.SelectedPower;
                playerControler.VariableCardModifier = timesPreformed;
                playerControler.PreformingAbility = true;
                playerControler.NextAction = false;
                foreach (IEnumerator action in abilities)
                {
                    playerControler.UnmodifiedAction = true;
                    yield return StartCoroutine(actionManager.PreformAction(action));
                    playerControler.UnmodifiedAction = false;
                    yield return new WaitUntil(() => playerControler.NextAction == true);
                    playerControler.NextAction = false;
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