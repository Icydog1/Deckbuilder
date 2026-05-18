public class CripplingPoison : Card
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


    public override void PrepareTop()
    {
        currentActions.Add(() => playerControler.ApplyConditions(new Condition[] { new Poison(5) }, "enemy"));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.ApplyConditions(new Condition[] { new Poison(2), new Strength(-2,5), new Speed(-2,5) }, "enemy"));
    }
}
