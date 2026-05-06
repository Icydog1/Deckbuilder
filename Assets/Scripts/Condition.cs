using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Condition : MonoBehaviour
{
    private string conditionName;
    public string Name { get { return conditionName; } }
    private int amount;
    public int Value { get { return amount; } set { amount = value; } }

    private int duration;
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

    protected System.Action[] plan;
    public System.Action[] Plan { get { return plan; } }

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

    public Condition(string name, int conditionValue, int conditionDuration, int conditionAddType, bool isStartOfTurnCondition, bool isShown, System.Action[] actionPlan = null, string conditionAbnormality = null)
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
    public virtual void OnGain()
    {

    }
    public virtual void OnLoss()
    {

    }
}

public class Strength : Condition
{
    public Strength(int conditionValue, int conditionDuration = -1, int addType = 1) : base("Strength", conditionValue, conditionDuration, addType, false, true) {}
}

public class Dexterity : Condition
{
    public Dexterity(int conditionValue, int conditionDuration = -1, int addType = 1) : base("Dexterity", conditionValue, conditionDuration, addType, false, true) {}
}

public class Speed : Condition
{
    public Speed(int conditionValue, int conditionDuration = -1, int addType = 1) : base("Speed", conditionValue, conditionDuration, addType, false, true) { }
}

public class  Finesse: Condition
{
    public Finesse(int conditionValue, int conditionDuration = -1, int addType = 1) : base("Finesse", conditionValue, conditionDuration, addType, false, true) { }
}

public class NaturalScaling: Condition
{
    public NaturalScaling(int conditionValue, int conditionDuration = -1, int addType = 3) : base("NaturalScaling", conditionValue, conditionDuration, addType, false, false) { }
}

public class DistanceSpeedBoost : Condition
{
    public DistanceSpeedBoost(int conditionValue, int conditionDuration = 1, int addType = 3) : base("DistanceSpeedBoost", conditionValue, conditionDuration, addType, false, false) { }
}
public class DistanceJump : Condition
{
    public DistanceJump(int conditionValue = -1, int conditionDuration = 1, int addType = 3) : base("DistanceJump", conditionValue, conditionDuration, addType, false, false) { }
}

public class Poison : Condition
{
    public Poison(int conditionValue, int conditionDuration = -1, int addType = 1) : base("Poison", conditionValue, conditionDuration, addType, true, true) { }
}

public class NextTurns : Condition
{
    public NextTurns(System.Action[] nextTurnsAction, int conditionValue = 0, int conditionDuration = 1, int addType = 1, string abnormality = "Delayed Gain") : base("NextTurns", conditionValue, conditionDuration, addType, true, true, nextTurnsAction, abnormality) { }
}

public class GainAbility : Condition
{
    protected Ability gainedAbility;
    public Ability GainedAbility { get { return gainedAbility; } }
    public GainAbility(Ability conditionAbility, int conditionValue = 0, int conditionDuration = 1, int addType = 2, string abnormality = "Ability") : base("Ability", conditionValue, conditionDuration, addType, true, true, null, abnormality)
    {
        gainedAbility = conditionAbility;
    }
    public override void OnGain()
    {
        GameObject.Find("Player").GetComponent<PlayerControler>().GainAbility(gainedAbility);
    }
    public override void OnLoss()
    {
        GameObject.Find("Player").GetComponent<PlayerControler>().LoseAbility(gainedAbility);
    }
}