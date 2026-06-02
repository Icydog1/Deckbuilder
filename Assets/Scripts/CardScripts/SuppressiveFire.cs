public class SuppressiveFire : Card
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
        currentActions.Add(() => playerControler.Attack(19, 5, 2));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Attack(5, 4, 2, 1, new Condition[] { new Speed(-15, 1) }));
    }
}


