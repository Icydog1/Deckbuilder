public class ChaoticEnergy : WizardCard
{
    public ChaoticEnergy() : base(2, 1, 1, 18) { }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(13)));
        currentActions.Add(new Action(() => playerControler.Block(7)));
        currentActions.Add(new Action(() => playerControler.Skill(9)));
        currentActions.Add(new Action(() => playerControler.Move(11)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Discard(Var.infinityValue)));
        currentActions.Add(new Action(() => playerControler.Draw(7)));
        currentActions.Add(new Action(() => playerControler.Exhausting(4)));

    }
}