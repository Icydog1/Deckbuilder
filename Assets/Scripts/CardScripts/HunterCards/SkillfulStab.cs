public class SkillfulStab : Card
{
    public SkillfulStab() : base(1, 1, 0) { }




    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(10)));
        currentActions.Add( new Action(() => playerControler.Draw(1)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Skill(10)));
    }
}
