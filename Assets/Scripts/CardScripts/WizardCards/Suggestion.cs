public class Suggestion : WizardCard
{
    public Suggestion() : base(0, 1, 1) { }




    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command("enemy", 3, 1)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(11)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Strength(-8,1),"enemy")));

    }
}
