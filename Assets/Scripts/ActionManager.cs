using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    protected PlayerControler playerControler;


    //private List<System.Action> actionQueue = new List<System.Action>();
    private List<IEnumerator> actionQueue = new List<IEnumerator>();
    private List<IEnumerator> planQueue = new List<IEnumerator>();
    private List<List<string>> planToQueue = new List<List<string>>();
    public List<string> PlanToList { get { return planToQueue[0]; } }

    //public List<System.Action> ActionQueue { get { return actionQueue; } set { actionQueue = value;} }
    private bool actionLoopRunning, planLoopRunning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();


        //actionQueue.Add(() => StartCoroutine(Test()));
        //actionCoroutines.Add(StartCoroutine(Test()));
        //QueueAction(Test());
        //actionCoroutine = Test();
        //StartActionLoop();
        //StartCoroutine(actionCoroutines[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //QueueAction(() => StartCoroutine(Test()));
            //actionCoroutines.Add(StartCoroutine(Test()));

            //QueueAction(() => StartCoroutine(Wait()));
            //actionCoroutines.Add(StartCoroutine(Wait()));
            //QueueAction(WaitUntilRewardSelected());

            StartCoroutine(PreformAction(Wait()));
        }
    }
    public IEnumerator PreformAction(IEnumerator action, List<string> planTo = null)
    {

        if (planTo != null)
        {
            planQueue.Add(action);
            planToQueue.Add(planTo);
            if (!planLoopRunning)
            {
                yield return StartCoroutine(StartPlanActionLoop());
            }
            yield return new WaitUntil(() => planLoopRunning == false);

        }
        else
        {
            actionQueue.Add(action);
            if (!actionLoopRunning)
            {
                yield return StartCoroutine(StartActionLoop());
            }
            yield return new WaitUntil(() => actionLoopRunning == false);

        }
        //Debug.Log("mabie error??");
        yield return null;
    }

    //public void QueueAction(IEnumerator action, bool isPlan = false)
    //{
    //    if (isPlan)
    //    {
    //        planQueue.Add(action);
    //        if (!planLoopRunning)
    //        {
    //            StartCoroutine(StartPlanActionLoop());
    //        }
    //    }
    //    else
    //    {
    //        actionQueue.Add(action);
    //        if (!actionLoopRunning)
    //        {
    //            StartCoroutine(StartActionLoop());
    //        }
    //    }

    //    //StartActionLoop();
    //}
    public IEnumerator StartPlanActionLoop()
    {
        //Debug.Log("started action loop");

        planLoopRunning = true;
        playerControler.IsPlanning = true;
        while (planQueue.Count > 0)
        {
            //Debug.Log("started plan");
            //Debug.Log(actionCoroutines.Count
            yield return StartCoroutine(planQueue[0]);
            //yield return new WaitForSeconds(0.5f);
            //Debug.Log("ended plan");

            //actionQueue[0]();

            planQueue.RemoveAt(0);
            planToQueue.RemoveAt(0);

        }
        playerControler.IsPlanning = false;
        planLoopRunning = false;
        yield return null;
    }

    public IEnumerator StartActionLoop()
    {
        actionLoopRunning = true;
        while (actionQueue.Count > 0)
        {
            if (planLoopRunning)
            {
                yield return new WaitUntil(() => planLoopRunning == false);
            }
            //Debug.Log(actionCoroutines.Count);
            yield return StartCoroutine(actionQueue[0]);
            //yield return new WaitForSeconds(0.5f);
            //actionQueue[0]();

            actionQueue.RemoveAt(0);
        }
        actionLoopRunning = false;
        yield return null;

    }

    public IEnumerator Test()
    {
        Debug.Log("test");
        yield return null;
    }
    public IEnumerator Wait()
    {
        Debug.Log("Wait");
        yield return new WaitForSeconds(1);
        Debug.Log("Wait done");
    }
}
