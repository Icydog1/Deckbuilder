public class FancyFootwork : Card
{
    public FancyFootwork() : base(1, 0, 1) { }

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Block(8)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(5)));
        currentActions.Add( new Action(() => playerControler.Attack(5)));
    }
}