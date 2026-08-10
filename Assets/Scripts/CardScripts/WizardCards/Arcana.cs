public class Arcana : WizardCard
{
    public Arcana() : base(0, 1, 2, 8) { }




    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(17)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Skill(26)));


    }
}
