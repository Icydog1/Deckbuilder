 public class Baracade : Card
{
    public Baracade() : base(2, 2, 1) { }

    public override void Start()
    {
        
        base.Start();
    }
    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Block(57)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new Speed(-5, 2))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Block(12)));
    }
}


