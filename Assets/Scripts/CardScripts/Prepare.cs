using System.Collections;

public class Prepare : Card
{
    public override void Start()
    {
        topCost = 1;
        bottomCost = 1;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override IEnumerator PrepareTop()
    {
        //yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new NextTurns(new System.Action[] { () => playerControler.GainTopEnergy(2), () => playerControler.Draw(1) })));

        yield return "Temp";

        //currentActions.Add(() => playerControler.ApplyCondition( new NextTurns(new System.Action[] { () => playerControler.GainTopEnergy(2), () => playerControler.Draw(1) })) );
    }

    public override IEnumerator PrepareBottom()
    {
        //currentActions.Add(() => playerControler.ApplyCondition(new NextTurns(new System.Action[] { () => playerControler.GainBottomEnergy(2), () => playerControler.Draw(1) })));

        yield return "Temp";
    }
}
