 public class FullPower : Card
{
    public FullPower() : base(1, 2, 2) { }




    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(20)));
        currentActions.Add( new Action(() => playerControler.Attack(20)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(35)));

    }
}
