public class IceBolt : WizardCard
{
    public IceBolt() : base(1, 1, 1, 9,5) { }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(7, 3, 1, 1, new Condition[] { new Speed(-11) })));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(22)));
    }
}
