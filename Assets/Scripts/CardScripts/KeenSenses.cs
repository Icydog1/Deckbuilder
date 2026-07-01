public class KeenSenses : Card
{
    public KeenSenses() : base(2, 1, 1) { }




    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Block(7)));
        currentActions.Add(new Action(() => playerControler.Draw(2)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Accuracy(3,2))));

    }
}
