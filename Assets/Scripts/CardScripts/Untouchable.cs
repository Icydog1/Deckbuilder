public class Untouchable : Card
{
    public Untouchable() : base(2, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new BlockPerMove(5, 1))));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(20,true)));
    }
}
