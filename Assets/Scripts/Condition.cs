using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class Condition
{
    protected ConditionEffects conditionEffects;
    protected string conditionName;
    public string ConditionName { get { return conditionName; } }
    protected string actionName;
    public string ActionName { get { return actionName; } }
    protected int amount;
    public int Value { get { return amount; } set { amount = value; } }

    protected int duration;
    public int Duration { get { return duration; } set { duration = value; } }

    //addType 0: new instance (unoffical)
    //addType 1: add values
    //addType 2: add durations
    //addType 3: override

    protected int addType;
    public int AddType { get { return addType; }}
    protected string effectedAction;
    public string EffectedAction { get { return effectedAction; } }
    protected bool isVisible;
    public bool IsVisible { get { return isVisible; } }
    protected bool isStartOfTurn;
    public bool IsStartOfTurn { get { return isStartOfTurn; } }

    protected Func<IEnumerator>[] plan;
    public Func<IEnumerator>[] Plan { get { return plan; } }

    protected string[] abnormality;
    public string[] Abnormality { get { return abnormality; } }
    protected string description;
    public string Description { get { return description; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Condition(string name, int conditionValue, int conditionDuration, int conditionAddType, bool isStartOfTurnCondition, string effectedActionType, bool isShown = true, string conditionDescription = null, string[] conditionAbnormality = null, Func<IEnumerator>[] actionPlan = null)
    {
        //Debug.Log("base Condition generated");
        conditionName = name;
        amount = conditionValue;
        duration = conditionDuration;
        addType = conditionAddType;
        effectedAction = effectedActionType;
        isVisible = isShown;
        isStartOfTurn = isStartOfTurnCondition;
        abnormality = conditionAbnormality;
        plan = actionPlan;
        actionName = name;
        description = conditionDescription;
    }
    public virtual IEnumerator OnGain(Figure figure)
    {
        //Debug.Log("Gaind Condition");

        yield return null;
    }
    public virtual IEnumerator OnLoss(Figure figure)
    {
        yield return null;
    }
    public Condition Clone()
    {
        return (Condition)this.MemberwiseClone();
    }
}

public class Strength : Condition
{
    public Strength(int conditionValue, int conditionDuration = -1) : base("Strength", conditionValue, conditionDuration, 1, false, "AttackValue") {}
}

public class Dexterity : Condition
{
    public Dexterity(int conditionValue, int conditionDuration = -1) : base("Dexterity", conditionValue, conditionDuration, 1, false, "BlockValue") {}
}

public class Speed : Condition
{
    public Speed(int conditionValue, int conditionDuration = -1) : base("Speed", conditionValue, conditionDuration, 1, false, "MoveValue") { }
}

public class Finesse: Condition
{
    public Finesse(int conditionValue, int conditionDuration = -1) : base("Finesse", conditionValue, conditionDuration, 1, false, "AbilityValue") { }
}
public class Accuracy : Condition
{
    public Accuracy(int conditionValue, int conditionDuration = -1) : base("Accuracy", conditionValue, conditionDuration, 1, false, "RangeValue") { }
}
public class NaturalScaling: Condition
{
    public NaturalScaling(int conditionValue, int conditionDuration = -1) : base("NaturalScaling", conditionValue, conditionDuration, 3, false, "All", false) { }
}

public class DistanceSpeedBoost : Condition
{
    public DistanceSpeedBoost(int conditionValue, int conditionDuration = 1) : base("DistanceSpeedBoost", conditionValue, conditionDuration, 3, false, "MoveValue", false) { }
}
public class DistanceJump : Condition
{
    public DistanceJump(int conditionValue = -1, int conditionDuration = 1) : base("DistanceJump", conditionValue, conditionDuration, 3, false, "All", false) { }
}

public class Poison : Condition
{
    public Poison(int conditionValue, int conditionDuration = -1) : base("Poison", conditionValue, conditionDuration, 1, true, "None", true) { }
}

public class NextTurns : Condition
{
    public NextTurns(Func<IEnumerator>[] nextTurnsAction, int conditionValue = Variables.gameNullValue, int conditionDuration = 1) : base("NextTurns", conditionValue, conditionDuration, 0, true, "All", true, null, new string[] { "Delayed Gain", "No Self Target Description" }, nextTurnsAction) { }
}
public class NextTurnBlock : Condition
{
    public NextTurnBlock(int conditionValue, int conditionDuration = 1) : base("NextTurnBlock", conditionValue, conditionDuration, 1, true, "None", true, "Next turn gain " + conditionValue + " block", new string[] { "Delayed Gain", "No Self Target Description" }) { }
}
public class NextTurnTopEnergy : Condition
{
    public NextTurnTopEnergy(int conditionValue, int conditionDuration = 1) : base("NextTurnTopEnergy", conditionValue, conditionDuration, 1, true, "None", true, "Next turn gain " + conditionValue + " top energy", new string[] { "Delayed Gain", "No Self Target Description" }) { }
}
public class NextTurnBottomEnergy : Condition
{
    public NextTurnBottomEnergy(int conditionValue, int conditionDuration = 1) : base("NextTurnBottomEnergy", conditionValue, conditionDuration, 1, true, "None", true, "Next turn gain " + conditionValue + " bottom energy", new string[] { "Delayed Gain", "No Self Target Description" }) { }
}
public class GainAbility : Condition
{
    protected Ability gainedAbility;
    public Ability GainedAbility { get { return gainedAbility; } }
    public GainAbility(Ability conditionAbility, int conditionDuration = 1) : base("Ability", Variables.gameNullValue, conditionDuration, 2, false, "All", true, null, new string[] { "Ability", "No Self Target Description" })
    {
        conditionEffects = GameObject.Find("ConditionEffects").GetComponent<ConditionEffects>();

        gainedAbility = conditionAbility;
    }
    public override IEnumerator OnGain(Figure figure)
    {
        //Debug.Log("gained condtion");
        yield return conditionEffects.StartCoroutine(GameObject.Find("Player").GetComponent<PlayerControler>().GainAbility(gainedAbility));
    }
    public override IEnumerator OnLoss(Figure figure)
    {
        yield return conditionEffects.StartCoroutine(GameObject.Find("Player").GetComponent<PlayerControler>().LoseAbility(gainedAbility));
    }
}
public class Vigor : Condition
{
    public Vigor(int conditionValue, int conditionDuration = -1) : base("Vigor", conditionValue, conditionDuration, 1, false, "AttackValue") { }
}
public class Burst : Condition
{
    public Burst(int conditionValue, int conditionDuration = -1) : base("Burst", conditionValue, conditionDuration, 1, false, "MoveValue") { }
}
public class BlockPerMove : Condition
{
    public BlockPerMove(int conditionValue = 1, int conditionDuration = 1) : base("Untouchable", conditionValue, conditionDuration, 1, true, "None", true, null, new string[] { "No Value Description", "No Self Target Description" } )
    {
        actionName = "Whenever you move a space gain " + conditionValue + " block";
    }
}

public class Flight : Condition
{
    public Flight(int conditionDuration = -1) : base("Flight", Variables.gameNullValue, conditionDuration, 2, false, "None", true) { }
    public override IEnumerator OnGain(Figure figure)
    {
        //Debug.Log("gained condtion");
        figure.CanFly = true;
        yield break;
    }
    public override IEnumerator OnLoss(Figure figure)
    {
        figure.CanFly = false;
        yield break;
    }
}
//public class StartOfTurnSlow : Condition
//{
//    public StartOfTurnSlow(int conditionValue, int conditionDuration = -1) : base("StartOfTurnSlow", conditionValue, conditionDuration, 1, true, "None", false) { }
//}

public class Stunned : Condition
{
    public Stunned(int conditionDuration = 1, bool isShown = true) : base("Stunned", Variables.gameNullValue, conditionDuration, 2, false, "All", isShown) { }
}
public class Summon : Condition
{
    public Summon() : base("Summon", Variables.gameNullValue, -1, 3, false, "None") { }
}
//public class StartOfTurnBlock : Condition
//{
//    public StartOfTurnBlock(int conditionValue, int conditionDuration = 1) : base("Next Turn Block", conditionValue, conditionDuration, 1, true, false) { }
//}