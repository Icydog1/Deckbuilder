using System.Collections;
using UnityEngine;

public class PlayerSummon : AIFigure
{
    public override void Awake()
    {
        team = 0;
        isEnemy = false;
        isPlayerSummon = true;
        base.Awake();

        playerControler.PlayerTurnStartedFuntions += ResetBlockOnPlayerTurn;


    }
    public override IEnumerator BaseStartTurn()
    {
        //Debug.Log(gameObject + " is takeing turn");
        turn++;
        yield return StartCoroutine(conditionEffects.StartOfTurnConditions(this));
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].IsStartOfTurn && conditions[i].Duration > 0)
            {
                conditions[i].Duration--;
                //Debug.Log("counted down " + conditions[i].Name + " to " + conditions[i].Duration);
            }
            if (conditions[i].IsStartOfTurn && conditions[i].Duration == 0)
            {
                //Debug.Log("removed " + conditions[i].ConditionName + "at start of turn");
                yield return StartCoroutine(conditions[i].OnLoss(this));
                conditions.RemoveAt(i);
                i--;
            }
        }
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
    }
    public void ResetBlockOnPlayerTurn(PlayerControler playerControler)
    {
        ResetBlock();
    }
}
