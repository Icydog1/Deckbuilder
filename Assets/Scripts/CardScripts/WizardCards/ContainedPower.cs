 public class ContainedPower : WizardCard
{
    public ContainedPower() : base(2, 1, 2, 7) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Block(14)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Vigor(19, 3))));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new ManaCapacity(13, 6))));
        currentActions.Add(new Action(() => playerControler.Exhausting(3)));

    }
}