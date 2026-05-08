using System.Collections;

public class ShiftingPower : Card
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
        yield return "Temp";

        currentActions.Add(() => playerControler.GainBottomEnergy(1));
        currentActions.Add(() => playerControler.Draw(1));

    }

    public override IEnumerator PrepareBottom()
    {
        yield return "Temp";

        currentActions.Add(() => playerControler.GainTopEnergy(1));
        currentActions.Add(() => playerControler.Draw(1));
    }
}
