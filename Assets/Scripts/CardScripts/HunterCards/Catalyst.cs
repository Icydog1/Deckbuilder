using System.Collections;
using System.Collections.Generic;

public class Catalyst : Card
{
    public Catalyst() : base(3, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => DoublePoison(),"Double target enemy's poison"));
        currentActions.Add(new Action(() => playerControler.Exhausting(2)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyConditions(new Condition[] { new Poison(7) }, "enemy")));
    }

    //custom fuctnion olny this script will use
    public IEnumerator DoublePoison()
    {
        actionManager.ActionStackNames.Push("DoublePoison");
        List<Figure> posibleTargets = playerControler.FindPosibleTargets("enemy", 1);
        playerControler.TargetsLeft = 1;
        while (playerControler.TargetsLeft > 0 && posibleTargets.Count > 0)
        {
            Figure targetedFigure = null;
            yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
            posibleTargets.Remove(targetedFigure);
            if (playerControler.TargetsLeft > 0)
            {
                playerControler.TargetsLeft--;
                yield return StartCoroutine(playerControler.ApplyConditionTo(new Poison(targetedFigure.GetValueOfCondition("Poison")), targetedFigure));
                //foreach (Condition condition in newConditions)
                //{
                //    yield return gameManager.StartCoroutine(targetedFigure.GainCondition(condition));
                //}
                if (playerControler.TargetsLeft <= 0)
                {
                    playerControler.EndAction();
                }
            }
        }
        //if (!playerControler.ActionEnded)
        {
            playerControler.EndAction();
        }
    }
}
