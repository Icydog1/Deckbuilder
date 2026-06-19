public class PoisonedDart : Card
{
    public PoisonedDart() : base(1, 1, 1) { }

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
        currentActions.Add( new Action(() => playerControler.Attack(3,3,1,1, new Condition[] { new Poison(3) })));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(10)));
        currentActions.Add( new Action(() => playerControler.Ability(10)));

    }
}
