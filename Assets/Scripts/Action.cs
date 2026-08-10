using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Action : MonoBehaviour
{
    protected ActionManager actionManager;
    protected PlayerControler playerControler;
    //protected Card card = null;
    public Func<IEnumerator> preformedAction;
    public Func<Figure, IEnumerator> preformedAction2;
    public string description = null;
    public bool multitarget;

    public Action(Func<Figure, IEnumerator> action, string descriptionOverride = null)
    {
        preformedAction2 = action;
        description = descriptionOverride;
        multitarget = true;
        LoadAction();
        //Debug.Log(targets.Count);
        //RefrenceStorage.playerControler.EffectedFigures.Add(RefrenceStorage.playerControler);
        //Debug.Log(targets.Count);
        //Debug.Log(targets[0]);
    }
    public Action(Func<IEnumerator> action, string descriptionOverride = null)
    {
        preformedAction = action;
        description = descriptionOverride;
        multitarget = false;
        LoadAction();
    }

    void LoadAction()
    {
        actionManager = RefrenceStorage.actionManager;
        playerControler = RefrenceStorage.playerControler;
    }



    public IEnumerator PreformAction(ActionPreformer actionPreformer = null, List<ActionDescription> planTo = null)
    {
        //if (actionPreformer is Card Card)
        //{
        //    card = Card;
        //}
        //else
        //{
        //    card = null;
        //}
        if (multitarget == false)
        {
            if (planTo != null)
            {
                //Debug.Log();
                yield return actionManager.StartCoroutine(actionManager.PreformAction(preformedAction(), planTo));
            }
            else
            {
                if (actionPreformer.StopPlaying == false)
                {
                    //Debug.Log("started action");
                    actionManager.ActionEnded = false;
                    yield return actionManager.StartCoroutine(actionManager.PreformAction(preformedAction()));
                    //Debug.Log("finished action");
                }
            }

        }
        else
        {
            if (planTo != null)
            {
                playerControler.UnmodifiedAction = true;
                yield return actionManager.StartCoroutine(actionManager.PreformAction(preformedAction2(playerControler), planTo));
                playerControler.UnmodifiedAction = false;
            }
            else
            {
                //ActionDescription copyDescription = playerControler.ActionsRemaining[0];
                if (actionPreformer.ActingFigures.Count == 0)
                {
                    playerControler.ActionsRemaining.RemoveAt(0);
                }
                else if (actionPreformer.ActingFigures.Count > 1)
                {
                    for (int i = 1; i < actionPreformer.ActingFigures.Count; i++)
                    {
                        //Debug.Log("not tested");
                        playerControler.ActionsRemaining.Insert(i, playerControler.ActionsRemaining[0]); //copyDescription.Clone()
                    }
                }
                playerControler.UpdatePlan();
                //Debug.Log(actingFigures.Count);
                foreach (Figure target in actionPreformer.ActingFigures.ToArray())
                {
                    //Debug.Log(target);
                    if (actionPreformer.StopPlaying == false)
                    {
                        actionManager.ActionEnded = false;
                        if (target.Exists)
                        {
                            actionManager.ActiveFigure = target;
                            //currentTarget = target;
                            yield return actionManager.StartCoroutine(actionManager.PreformAction(preformedAction2(target), planTo));
                        }
                        else
                        {
                            actionPreformer.ActingFigures.Remove(target);
                        }

                        playerControler.EndAction();
                    }
                }
                actionManager.ActiveFigure = playerControler;

            }
        }
    }
}