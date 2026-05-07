using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    protected PlayerControler playerControler;


    private List<System.Action> actionQueue = new List<System.Action>();
    private List<IEnumerator> actionCoroutines = new List<IEnumerator>();

    public List<System.Action> ActionQueue { get { return actionQueue; } set { actionQueue = value;} }
    private bool loopRunning;
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

            QueueAction(Wait());
        }
    }
    public IEnumerator PreformAction(IEnumerator action)
    {
        actionCoroutines.Add(action);
        if (!loopRunning)
        {
            yield return StartCoroutine(StartActionLoop());
        }
        Debug.Log("mabie error??");
        yield return null;
    }

    public void QueueAction(IEnumerator action)
    {
        actionCoroutines.Add(action);
        if (!loopRunning)
        {
            StartCoroutine(StartActionLoop());
        }
        //StartActionLoop();
    }

    public IEnumerator StartActionLoop()
    {
        loopRunning = true;
        while (actionCoroutines.Count > 0)
        {
            Debug.Log(actionCoroutines.Count);
            yield return StartCoroutine(actionCoroutines[0]);
            //actionQueue[0]();

            actionCoroutines.RemoveAt(0);
        }
        loopRunning = false;
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
