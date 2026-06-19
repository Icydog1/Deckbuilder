public class FlameWave : Card
{
    public FlameWave() : base(2, 1, 1) { }

    public override void Start()
    {
        
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(9,4,Variables.gameInfinityValue,1,null,false,false)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Ability(23)));
    }
}


