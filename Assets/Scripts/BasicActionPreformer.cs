using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicActionPreformer : ActionPreformer
{
    private PlayerControler playerControler;
    private List<Action> storedActions = new List<Action>();
    //public List<Action> Actions { get { return storedActions; } }
    private List<ActionDescription> description = new List<ActionDescription>();
    public BasicActionPreformer(List<Action> actions)
    {
        storedActions = actions;
        //Debug.Log("action prefomer spawned");
        playerControler = RefrenceStorage.playerControler;
        RefrenceStorage.gameManager.StartCoroutine(PreformActions(true));
    }
    //preform stored actions ore get a description
    public IEnumerator PreformActions(bool isPlanning = false)
    {
        if (isPlanning)
        {
            //make a description of stored actions
            description.Clear();
            foreach (Action action in storedActions)
            {

                yield return RefrenceStorage.gameManager.StartCoroutine(action.PreformAction(this, description));

            }
        }
        else
        {
            //preforme stored actions
            playerControler.ActionsRemaining = new List<ActionDescription>(description);
            playerControler.SpecialPreformingAction = true;
            playerControler.ActiveActionPreformer = this;
            RefrenceStorage.actionManager.ActionEnded = false;
            foreach (Action action in storedActions)
            {
                yield return RefrenceStorage.gameManager.StartCoroutine(action.PreformAction(this, null));

            }
            StopCommanding();
            playerControler.SpecialPreformingAction = false;
        }
    }
}
