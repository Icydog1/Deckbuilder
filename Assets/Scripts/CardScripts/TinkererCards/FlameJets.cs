public class FlameJets : Card
{
    public FlameJets() : base(2, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(9,4,Var.infinityValue)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Skill(11)));
        currentActions.Add( new Action(() => playerControler.Draw(1)));
    }
}


