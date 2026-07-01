 public class ShatteringBlow : Card
{
    public ShatteringBlow() : base(1, 2, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(20, 1, 1, 1, new Condition[] { new Strength(-10, 1) } )));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(15)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new Strength(10, 1))));

    }
}
