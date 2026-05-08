using System.Collections;
public class Baracade : Card
{
    public override void Start()
    {
        topCost = 2;
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

        currentActions.Add(() => playerControler.Block(50));
        currentActions.Add(() => playerControler.ApplyCondition(new Speed(-5, 2)));
    }

    public override IEnumerator PrepareBottom()
    {
        yield return "Temp";

        currentActions.Add(() => playerControler.Block(10));
    }
}
