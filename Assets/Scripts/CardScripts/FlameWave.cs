public class FlameWave : Card
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
        currentActions.Add(() => playerControler.Attack(12,4,-1,1,null,false,false));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Ability(23));
    }
}


