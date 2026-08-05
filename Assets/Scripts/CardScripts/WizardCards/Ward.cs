public class Ward : WizardCard
{
    public Ward() : base(1, 1, 1, 7) { }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Block(29)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(10)));
        currentActions.Add(new Action(() => playerControler.GainMana(8)));
    }
}