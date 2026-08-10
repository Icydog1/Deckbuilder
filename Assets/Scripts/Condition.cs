using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class Condition
{
    //internal name
    protected string conditionName;
    public string ConditionName { get { return conditionName; } }
    //name shown to player
    protected string actionName;
    public string ActionName { get { return actionName; } }
    //override of what player sees for while being planned
    protected string plannedDescription;
    public string PlannedDescription { get { return plannedDescription; } }
    //override of what player sees for while active
    protected string activeDescription;
    public string ActiveDescription { get { return activeDescription; } }
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
    protected List<ActionDescription> planDescription;
    public List<ActionDescription> PlanDescription { get { return planDescription; } }

    protected string[] abnormality;
    public string[] Abnormality { get { return abnormality; } }

    public Condition(string name, int conditionValue, int conditionDuration, int conditionAddType, bool isStartOfTurnCondition, string effectedActionType, bool isShown = true, string shownName = null, string[] conditionAbnormality = null, Func<IEnumerator>[] actionPlan = null)
    {
        //Debug.Log("base Condition generated");
        conditionName = name;
        if (shownName == null)
        {
            actionName = name;
        }
        else
        {
            actionName = shownName;
        }
        amount = conditionValue;
        duration = conditionDuration;
        addType = conditionAddType;
        effectedAction = effectedActionType;
        isVisible = isShown;
        isStartOfTurn = isStartOfTurnCondition;
        abnormality = conditionAbnormality;
        plan = actionPlan;
        //planDescription = conditionDescription;
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
    public Strength(int conditionValue, int conditionDuration = Var.infinityValue) : base("Strength", conditionValue, conditionDuration, 1, false, "AttackValue") {}
}

public class Dexterity : Condition
{
    public Dexterity(int conditionValue, int conditionDuration = Var.infinityValue) : base("Dexterity", conditionValue, conditionDuration, 1, false, "BlockValue") {}
}

public class Speed : Condition
{
    public Speed(int conditionValue, int conditionDuration = Var.infinityValue) : base("Speed", conditionValue, conditionDuration, 1, false, "MoveValue") { }
}

public class Finesse: Condition
{
    public Finesse(int conditionValue, int conditionDuration = Var.infinityValue) : base("Finesse", conditionValue, conditionDuration, 1, false, "SkillValue") { }
}
public class Accuracy : Condition
{
    public Accuracy(int conditionValue, int conditionDuration = Var.infinityValue) : base("Accuracy", conditionValue, conditionDuration, 1, false, "RangeValue") { }
}
public class NaturalScaling: Condition
{
    public NaturalScaling() : base("NaturalScaling", Var.nullValue, Var.infinityValue, 3, false, "All", false) { }
}

public class DistanceSpeedBoost : Condition
{
    public DistanceSpeedBoost(int conditionValue, int conditionDuration = 1) : base("DistanceSpeedBoost", conditionValue, conditionDuration, 3, false, "MoveValue", false) { }
}
public class DistanceJump : Condition
{
    public DistanceJump(int conditionDuration = 1) : base("DistanceJump", Var.nullValue, conditionDuration, 3, false, "All", false) { }
}

public class Poison : Condition
{
    public Poison(int conditionValue) : base("Poison", conditionValue, Var.infinityValue, 1, true, "None", true) { }
}
public class Thorns : Condition
{
    public Thorns(int conditionValue, int conditionDuration = Var.infinityValue) : base("Thorns", conditionValue, conditionDuration, 1, false, "None") { }
}

public class NextTurns : Condition
{
    public NextTurns(Func<IEnumerator>[] nextTurnsAction, int conditionDuration = 1) : base("NextTurns", Var.nullValue, conditionDuration, 0, true, "All", true, null, new string[] { "Delayed Gain", "No Self Target Description" }, nextTurnsAction) { }
    public override IEnumerator OnGain(Figure figure)
    {
        planDescription = new List<ActionDescription>();
        //Debug.Log("gained condtion");
        foreach (Func<IEnumerator> action in plan)
        {
            yield return RefrenceStorage.conditionEffects.StartCoroutine(RefrenceStorage.actionManager.PreformAction(action(), planDescription));
        }
    }
}

public class StartOfTurnBlock : Condition
{
    public StartOfTurnBlock(int conditionValue, int conditionDuration = 1) : base("StartOfTurnBlock", conditionValue, conditionDuration, 1, true, "None", true, "Start of turn block", new string[] { "No Self Target Description" }) 
    {
        //actionName = "Start of turn gain " + conditionValue + " block";
        //actionName = "Start of turn gain " + conditionValue + " block";
    }
}

public class NextTurnTopEnergy : Condition
{
    public NextTurnTopEnergy(int conditionValue, int conditionDuration = 1) : base("NextTurnTopEnergy", conditionValue, conditionDuration, 1, true, "None", true, "Start of turn top energy", new string[] { "No Self Target Description" }) { }
}
public class NextTurnBottomEnergy : Condition
{
    public NextTurnBottomEnergy(int conditionValue, int conditionDuration = 1) : base("NextTurnBottomEnergy", conditionValue, conditionDuration, 1, true, "None", true, "Start of turn bottom energy", new string[] {"No Self Target Description" }) { }
}
public class NextTurnCards : Condition
{
    public NextTurnCards(int conditionValue, int conditionDuration = 1) : base("NextTurnCards", conditionValue, conditionDuration, 1, true, "None", true, "Start of turn cards", new string[] { "No Self Target Description" }) { }
}
public class GainAbility : Condition
{
    protected Ability gainedAbility;
    public Ability GainedAbility { get { return gainedAbility; } }
    public GainAbility(Ability conditionAbility, int conditionDuration = 1) : base("Ability", Var.nullValue, conditionDuration, 0, false, "All", true, null, new string[] { "Ability", "No Self Target Description" })
    {
        //conditionEffects = RefrenceStorage.conditionEffects;
        gainedAbility = conditionAbility;
    }
    public override IEnumerator OnGain(Figure figure)
    {
        //Debug.Log("gained condtion");
        yield return RefrenceStorage.conditionEffects.StartCoroutine(GameObject.Find("Player").GetComponent<PlayerControler>().GainAbility(gainedAbility));
    }
    public override IEnumerator OnLoss(Figure figure)
    {
        yield return RefrenceStorage.conditionEffects.StartCoroutine(GameObject.Find("Player").GetComponent<PlayerControler>().LoseAbility(gainedAbility));
    }
}
public class Vigor : Condition
{
    public Vigor(int conditionValue, int conditionDuration = Var.infinityValue) : base("Vigor", conditionValue, conditionDuration, 1, false, "AttackValue") { }
}
public class Burst : Condition
{
    public Burst(int conditionValue, int conditionDuration = Var.infinityValue) : base("Burst", conditionValue, conditionDuration, 1, false, "MoveValue") { }
}
public class BlockPerMove : Condition
{
    public BlockPerMove(int conditionValue, int conditionDuration = 1) : base("Untouchable", conditionValue, conditionDuration, 1, false, "None", true, null, new string[] { "No Value Description", "No Self Target Description" } )
    {
        actionName = "Whenever you move a space gain " + conditionValue + " block";
    }
}

public class Flight : Condition
{
    public Flight(int conditionDuration = Var.infinityValue) : base("Flight", Var.nullValue, conditionDuration, 2, false, "None", true) { }
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
    public Stunned(int conditionDuration = 1) : base("Stunned", Var.nullValue, conditionDuration, 2, false, "All", true) { }
}
public class Summon : Condition
{
    public Summon() : base("Summon", Var.nullValue, Var.infinityValue, 3, false, "None") { }
}
public class ManaCapacity : Condition
{
    public ManaCapacity(int conditionValue, int conditionDuration = Var.infinityValue, bool isShown = true) : base("Mana Capacity", conditionValue, conditionDuration, 1, false, "None")
    { }
    public override IEnumerator OnGain(Figure figure)
    {
        if (figure is PlayerControler effectedPlayer)
        {
            effectedPlayer.ManaCapacity += amount;
            effectedPlayer.Mana += amount;
        }
        yield break;
    }
    public override IEnumerator OnLoss(Figure figure)
    {
        if (figure is PlayerControler effectedPlayer)
        {
            effectedPlayer.ManaCapacity -= amount;
            //if (effectedPlayer.Mana > effectedPlayer.ManaCapacity)
            //{
            //    effectedPlayer.Mana = Mathf.Max(effectedPlayer.Mana-amount, effectedPlayer.ManaCapacity);
            //}
            //effectedPlayer.Mana -= amount;

        }
        yield break;
    }
}

//public class StartOfTurnBlock : Condition
//{
//    public StartOfTurnBlock(int conditionValue, int conditionDuration = 1) : base("Next Turn Block", conditionValue, conditionDuration, 1, true, false) { }
//}