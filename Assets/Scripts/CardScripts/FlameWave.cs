public class FlameWave : Card
{
    public FlameWave() : base(2, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(9,4,Variables.gameInfinityValue,1,null,false,true)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Skill(11)));
        currentActions.Add( new Action(() => playerControler.Draw(1)));
    }
}


