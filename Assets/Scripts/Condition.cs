using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Condition : MonoBehaviour
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
    protected bool isVisible;
    public bool IsVisible { get { return isVisible; } }
    protected bool isStartOfTurn;
    public bool IsStartOfTurn { get { return isStartOfTurn; } }

    protected Func<IEnumerator>[] plan;
    public Func<IEnumerator>[] Plan { get { return plan; } }

    protected string abnormality;
    public string Abnormality { get { return abnormality; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Condition(string name, int conditionValue, int conditionDuration, int conditionAddType, bool isStartOfTurnCondition, bool isShown, Func<IEnumerator>[] actionPlan = null, string conditionAbnormality = null)
    {
        conditionName = name;
        amount = conditionValue;
        duration = conditionDuration;
        addType = conditionAddType;
        isVisible = isShown;
        isStartOfTurn = isStartOfTurnCondition;
        abnormality = conditionAbnormality;
        plan = actionPlan;
    }
    public virtual IEnumerator OnGain()
    {
        yield return null;
    }
    public virtual IEnumerator OnLoss()
    {
        yield return null;
    }
}

public class Strength : Condition
{
    public Strength(int conditionValue, int conditionDuration = -1) : base("Strength", conditionValue, conditionDuration, 1, false, true) {}
}

public class Dexterity : Condition
{
    public Dexterity(int conditionValue, int conditionDuration = -1) : base("Dexterity", conditionValue, conditionDuration, 1, false, true) {}
}

public class Speed : Condition
{
    public Speed(int conditionValue, int conditionDuration = -1) : base("Speed", conditionValue, conditionDuration, 1, false, true) { }
}

public class  Finesse: Condition
{
    public Finesse(int conditionValue, int conditionDuration = -1) : base("Finesse", conditionValue, conditionDuration, 1, false, true) { }
}

public class NaturalScaling: Condition
{
    public NaturalScaling(int conditionValue, int conditionDuration = -1) : base("NaturalScaling", conditionValue, conditionDuration, 3, false, false) { }
}

public class DistanceSpeedBoost : Condition
{
    public DistanceSpeedBoost(int conditionValue, int conditionDuration = 1) : base("DistanceSpeedBoost", conditionValue, conditionDuration, 3, false, false) { }
}
public class DistanceJump : Condition
{
    public DistanceJump(int conditionValue = -1, int conditionDuration = 1) : base("DistanceJump", conditionValue, conditionDuration, 3, false, false) { }
}

public class Poison : Condition
{
    public Poison(int conditionValue, int conditionDuration = -1) : base("Poison", conditionValue, conditionDuration, 1, true, true) { }
}

public class NextTurns : Condition
{
    public NextTurns(Func<IEnumerator>[] nextTurnsAction, int conditionValue = 0, int conditionDuration = 1) : base("NextTurns", conditionValue, conditionDuration, 0, true, true, nextTurnsAction, "Delayed Gain") { }
}

public class GainAbility : Condition
{
    protected Ability gainedAbility;
    public Ability GainedAbility { get { return gainedAbility; } }
    public GainAbility(Ability conditionAbility, int conditionValue = 0, int conditionDuration = 1) : base("Ability", conditionValue, conditionDuration, 2, false, true, null, "Ability")
    {
        conditionEffects = GameObject.Find("ConditionEffects").GetComponent<ConditionEffects>();

        gainedAbility = conditionAbility;
    }
    public override IEnumerator OnGain()
    {
        //Debug.Log("gained condtion");
        yield return conditionEffects.StartCoroutine(GameObject.Find("Player").GetComponent<PlayerControler>().GainAbility(gainedAbility));
    }
    public override IEnumerator OnLoss()
    {
        yield return conditionEffects.StartCoroutine(GameObject.Find("Player").GetComponent<PlayerControler>().LoseAbility(gainedAbility));
    }
}
public class Vigor : Condition
{
    public Vigor(int conditionValue, int conditionDuration = -1, int addType = 1) : base("Vigor", conditionValue, conditionDuration, addType, false, true) { }
}

public class BlockPerMove : Condition
{
    public BlockPerMove(int conditionValue = 1, int conditionDuration = 1, int addType = 1) : base("Untouchable", conditionValue, conditionDuration, addType, true, true)
    {
        actionName = "Whenever you move a space gain " + conditionValue + " block";
    }
}