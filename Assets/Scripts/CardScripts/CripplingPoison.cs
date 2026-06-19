public class CripplingPoison : Card
{
	public CripplingPoison() : base(1, 1, 1) { }

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
        currentActions.Add( new Action(() => playerControler.ApplyConditions(new Condition[] { new Poison(5) }, "enemy")));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.ApplyConditions(new Condition[] { new Poison(2), new Strength(-2,5), new Speed(-2,5) }, "enemy")));
    }
}
