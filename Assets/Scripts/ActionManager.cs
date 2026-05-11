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
    private Stack<IEnumerator> planStack = new Stack<IEnumerator>();
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
    private Queue<IEnumerator> preparedQueue = new Queue<IEnumerator>();

    private Stack<List<string>> planToStack = new Stack<List<string>>();
    //public List<string> planToList = new List<string>();
    public List<string> PlanToList { get { return planToStack.Peek(); } }//set { planToStack[planToStack.Count - 1] = value; } }
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
    public IEnumerator PreformAction(IEnumerator action, List<string> planTo = null)
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
        actionQueue.Enqueue(action);

    }
    public IEnumerator PreformFirstInQueue()
    {
        yield return StartCoroutine(actionQueue.Dequeue());
    }
    public void PrepareAction(IEnumerator action)
    {
        //Debug.Log("queued action");

        preparedQueue.Enqueue(action);
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

}
