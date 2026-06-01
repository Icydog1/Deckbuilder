using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionManager : MonoBehaviour
{
    protected PlayerControler playerControler;


    //private List<Func<IEnumerator>> actionStack = new List<Func<IEnumerator>>();
    private Stack<IEnumerator> actionStack = new Stack<IEnumerator>();
    private Stack<string> actionStackNames = new Stack<string>();
    public Stack<string> ActionStackNames { get { return actionStackNames; } set { actionStackNames = value;} }


    private Stack<IEnumerator> planStack = new Stack<IEnumerator>();
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
    private Queue<IEnumerator> preparedQueue = new Queue<IEnumerator>();

    private Stack<List<Action>> planToStack = new Stack<List<Action>>();
    //public List<string> planToList = new List<string>();
    public List<Action> PlanToList { get { return planToStack.Peek(); } }//set { planToStack[planToStack.Count - 1] = value; } }
    private bool preformingActions;

    //private IEnumerator test;
    //private Func<IEnumerator> test2;

    //public List<Func<IEnumerator>> ActionStack { get { return actionStack; } set { actionStack = value;} }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();

        //test = Test();
        //test = Test();
        //test2 = Test;
        //test2 = () => Test(2);

        //actionStack.Add(() => StartCoroutine(Test()));
        //actionCoroutines.Add(StartCoroutine(Test()));
        //StackAction(Test());
        //actionCoroutine = Test();
        //StartActionLoop();
        //StartCoroutine(actionCoroutines[0]);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator PreformAction(IEnumerator action, List<Action> planTo = null)
    {
        if (!preformingActions)
        {
            preformingActions = true;
        }
        if (planTo != null)
        {
            //Debug.Log("About to plan ablility");
            planStack.Push(action);
            planToStack.Push(planTo);
            playerControler.IsPlanning = true;
            yield return StartCoroutine(planStack.Pop());
            //planStack.RemoveAt(planStack.Count-1);
            planToStack.Pop(); //RemoveAt(planToStack.Count - 1);
            if (planToStack.Count == 0)
            {
                playerControler.IsPlanning = false;

            }
        }
        else
        {
            actionStack.Push(action);
            yield return StartCoroutine(actionStack.Pop());

            //actionStack.Add(action);
            //yield return StartCoroutine(actionStack[actionStack.Count - 1]);
            //actionStack.RemoveAt(actionStack.Count - 1);
        }
        if (actionStack.Count == 0 && planStack.Count == 0)
        {
            if (actionQueue.Count > 0)
            {
                yield return StartCoroutine(PreformFirstInQueue());
            }
            else if (preparedQueue.Count > 0)
            {
                yield return StartCoroutine(PreformPreparedActions());
            }
            else
            {
                preformingActions = false;
            }
        }
        //Debug.Log("mabie unnessasery");
        //yield return null;
    }
    public void QueueAction(IEnumerator action)
    {
        if (!preformingActions)
        {
            //Debug.Log("added action to queue");
            actionQueue.Enqueue(action);
        }
        else
        {
            //Debug.Log("started queue");
            StartCoroutine(PreformAction(action));
        }
    }
    public IEnumerator PreformFirstInQueue()
    {
        //Debug.Log("preformed one action in queue");
        yield return PreformAction(actionQueue.Dequeue());
    }
    public void PrepareAction(IEnumerator action)
    {
        //Debug.Log("queued action");
        if (!preformingActions)
        {
            //Debug.Log("added action to queue");
            preparedQueue.Enqueue(action);
        }
        else
        {
            //Debug.Log("started queue");
            StartCoroutine(PreformAction(action));
        }
    }
    public IEnumerator PreformPreparedActions()
    {
        //Debug.Log("ran perpared actions");
        while (preparedQueue.Count > 0)
        {
            yield return StartCoroutine(preparedQueue.Dequeue());
        }
    }

    //old loops
    //public IEnumerator PreformAction(IEnumerator action, List<string> planTo = null)
    //{
    //    if (planTo != null)
    //    {
    //        Debug.Log("About to plan ablility");
    //        planStack.Add(action);
    //        planToStack.Add(planTo);

    //        if (!planLoopRunning)
    //        {
    //            yield return StartCoroutine(StartPlanActionLoop());
    //        }
    //        yield return new WaitUntil(() => planLoopRunning == false);

    //    }
    //    else
    //    {
    //        actionStack.Add(action);
    //        if (!actionLoopRunning)
    //        {
    //            yield return StartCoroutine(StartActionLoop());
    //        }
    //        yield return new WaitUntil(() => actionLoopRunning == false);

    //    }
    //    //Debug.Log("mabie error??");
    //    yield return null;
    //}

    //public IEnumerator StartPlanActionLoop()
    //{
    //    //Debug.Log("started action loop");

    //    planLoopRunning = true;
    //    playerControler.IsPlanning = true;
    //    while (planStack.Count > 0)
    //    {
    //        //Debug.Log("started plan");
    //        //Debug.Log(actionCoroutines.Count
    //        yield return StartCoroutine(planStack[0]);
    //        //yield return new WaitForSeconds(0.5f);
    //        //Debug.Log("ended plan");

    //        //actionStack[0]();

    //        planStack.RemoveAt(0);
    //        planToStack.RemoveAt(0);

    //    }
    //    playerControler.IsPlanning = false;
    //    planLoopRunning = false;
    //    yield return null;
    //}

    //public IEnumerator StartActionLoop()
    //{
    //    actionLoopRunning = true;
    //    while (actionStack.Count > 0)
    //    {
    //        if (planLoopRunning)
    //        {
    //            yield return new WaitUntil(() => planLoopRunning == false);
    //        }
    //        //Debug.Log(actionCoroutines.Count);
    //        yield return StartCoroutine(actionStack[0]);
    //        //yield return new WaitForSeconds(0.5f);
    //        //actionStack[0]();

    //        actionStack.RemoveAt(0);
    //    }
    //    actionLoopRunning = false;
    //    yield return null;

    //}
    //public void test()
    //{
    //    Action test = new Action("Attack",new List<ActionModifier>() { new ActionModifier("start ",1, " end") });
    //}

}


public class Action : MonoBehaviour
{
    private string actionName;
    public string ActionName { get { return actionName; } set { actionName = value; } }

    private List<ActionModifier> actionModifiers;
    public List<ActionModifier> ActionModifiers { get { return actionModifiers; } set { actionModifiers = value; } }

    public Action(string name, List<ActionModifier> modifiers)
    {
        actionName = name;
        actionModifiers = modifiers;
    }

    public string GetDescription()
    {
        //return actionName;

        string description = string.Empty;
        foreach (ActionModifier modifier in actionModifiers)
        {
            description += modifier.InitialDescription;
            if (modifier.BaseValue != 1000000)
            {
                description += modifier.ModifiedValue;
            }
            description += modifier.FinalDescription;
        }
        return description;

    }
}
public class ActionModifier : MonoBehaviour
{
    private string initialDescription;
    public string InitialDescription { get { return initialDescription; } set { initialDescription = value; } }
    private int modifiedValue;
    public int ModifiedValue { get { return modifiedValue; } set { modifiedValue = value; } }
    private int baseValue;
    public int BaseValue { get { return baseValue; } set { baseValue = value; } }
    private string finalDescription;
    public string FinalDescription { get { return finalDescription; } set { finalDescription = value; } }
    private string type;
    public string Type { get { return type; } set { type = value; } }
    private Figure figure;
    public ActionModifier(Figure actingFigure, string startDescription = null, int value = 1000000, string endDescription = null, string valueType = null)
    {
        initialDescription = startDescription;
        finalDescription = endDescription;
        baseValue = value;
        type = valueType;
        figure = actingFigure;
        UpdateValue();
    }
    public void UpdateValue()
    {
        if (baseValue != 1000000)
        {
            switch (Type)
            {
                case "Block":
                    //Debug.Log("block");
                    //Debug.Log("base value " + baseValue);
                    modifiedValue = RefrenceStorage.conditionEffects.ModifyBlock(figure, baseValue);
                    //Debug.Log("modified value " + modifiedValue);

                    break;
                case "Attack":
                    //Debug.Log("Attack");
                    //Debug.Log("base value " + baseValue);
                    modifiedValue = RefrenceStorage.conditionEffects.ModifyAttack(figure, baseValue);
                    //Debug.Log("modified value " + modifiedValue);

                    break;
                case "Move":
                    //Debug.Log("Attack");
                    //Debug.Log("base value " + baseValue);
                    modifiedValue = RefrenceStorage.conditionEffects.ModifyMove(figure, baseValue);
                    //Debug.Log("modified value " + modifiedValue);
                    break;
                case "Ability":
                    //Debug.Log("Attack");
                    modifiedValue = RefrenceStorage.conditionEffects.ModifyAbility(figure, baseValue);
                    break;
                default:
                    //Debug.Log("Default");
                    modifiedValue = baseValue;
                    break;
            }
        }
        else
        {
            modifiedValue = baseValue;
        }

    }
}


