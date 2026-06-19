public class ShiftingPower : Card
{
    public ShiftingPower() : base(3, 1, 1) { }

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
        currentActions.Add( new Action(() => playerControler.GainBottomEnergy(1)));
        currentActions.Add( new Action(() => playerControler.Draw(1)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.GainTopEnergy(1)));
        currentActions.Add( new Action(() => playerControler.Draw(1)));
    }
}
