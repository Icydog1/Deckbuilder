public class SuppressiveFire : Card
{
    public SuppressiveFire() : base(3, 1, 1) { }

    public override void Start()
    {
        
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(13, 5, 2)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Attack(5, 1, 2, 1, new Condition[] { new Speed(-15, 1) })));
    }

}


