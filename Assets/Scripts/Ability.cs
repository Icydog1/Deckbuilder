using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected int cost;
    protected int abilityValue, timesPreformed;
    private int maxTimes;
    private bool isUsed;
    private List<System.Action> abilities = new List<System.Action>();
    protected List<string> description = new List<string>();

    private AbilityUI abilityUI;
    public AbilityUI AbilityUI { get { return abilityUI; } set { abilityUI = value; } }

    protected PlayerControler playerControler;
    protected MouseManager mouseManager;
    protected AbilityManager abilityManager;

    public int Cost { get { return cost; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();

        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();
        //abilityUI = transform.Find("AbilityUI").GetComponent<AbilityUI>();
        PlayerControler.PlayerTurnStarted += resetAbilityCooldown;
    }
    public void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void resetAbilityCooldown(PlayerControler playerControler)
    {
        isUsed = false;
        abilityUI.DisplayUsed(false);
    }

    public Ability(int abilityCost, List<System.Action> preformedAbilities)
    {
        abilities = preformedAbilities;
        cost = abilityCost;
        Awake();
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
        if (playerControler.CanPreformAbilities && !isUsed)
        {
            timesPreformed = Mathf.FloorToInt((float)abilitiesPointsSpent / (float)cost);
            if (timesPreformed >= 1)
            {
                isUsed = true;
                abilityUI.DisplayUsed(true);
                mouseManager.MouseOffObject(abilityUI.gameObject);
                abilityManager.AbilityPower -= timesPreformed * cost;
                abilityManager.SelectedPower = abilityManager.SelectedPower;
                playerControler.VariableCardModifier = timesPreformed;
                playerControler.PreformingAbility = true;
                foreach (System.Action action in abilities)
                {
                    action();
                    yield return new WaitUntil(() => playerControler.NextAction == true);
                    playerControler.NextAction = false;
                }
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