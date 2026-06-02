 public class Baracade : Card
{
    protected override int rarity => 2;

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


    public override void PrepareTop()
    {
        currentActions.Add(() => playerControler.Block(57));
        currentActions.Add(() => playerControler.ApplyCondition(new Speed(-5, 2)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Block(12));
    }
}


