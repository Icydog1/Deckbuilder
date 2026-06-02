public class Untouchable : Card
{
    protected override int rarity => 2;

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
        currentActions.Add(() => playerControler.ApplyCondition(new BlockPerMove(5, 1)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Move(20,true));
    }
}
