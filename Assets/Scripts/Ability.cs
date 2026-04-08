using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected int cost;
    protected int abilityValue, timesPreformed;
    private int maxTimes;
    private List<System.Action> abilities = new List<System.Action>();
    protected List<string> description = new List<string>();

    private AbilityUI abilityUI;
    protected PlayerControler playerControler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        abilityUI = transform.Find("AbilityUI").GetComponent<AbilityUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Ability(int abilityCost, List<System.Action> preformedAbilities)
    {
        abilities = preformedAbilities;
        cost = abilityCost;
    }
    
    public void UpdateDiscription(int abilitiesPointsSpent)
    {

        description.Clear();
        playerControler.IsPlanning = true;
        playerControler.PlanDescription = description;
        timesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
        playerControler.VariableCardModifier = timesPreformed;
        foreach (System.Action action in abilities)
        {
            action();
        }
        abilityUI.DisplayText(description);
        playerControler.IsPlanning = false;

    }

    public IEnumerator PreformAbility(int abilitiesPointsSpent)
    {
        timesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
        playerControler.VariableCardModifier = timesPreformed;
        foreach (System.Action action in abilities)
        {
            action();
            yield return new WaitUntil(() => playerControler.NextAction == true);
            playerControler.NextAction = false;
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