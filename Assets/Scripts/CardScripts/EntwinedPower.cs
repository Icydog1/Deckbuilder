public class EntwinedPower : Card
{
    protected override int rarity => 2;

    private int additionalCost = 1;
    public override void Start()
    {
        topCost = 1;
        bottomCost = 1;
        additionalTopDescription = "Cost: " + additionalCost + " bottom energy";
        additionalBottomDescription = "Cost: " + additionalCost + " top energy";

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        currentActions.Add(() => playerControler.Draw(3));
        currentActions.Add(() => playerControler.Attack(15));
        currentActions.Add(() => playerControler.Ability(15));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Draw(3));
        currentActions.Add(() => playerControler.Move(15));
        currentActions.Add(() => playerControler.Block(15));
    }


    public override void AttemptToPlayTop()
    {
        if (playerControler.TopEnergy >= topCost && playerControler.BottomEnergy >= additionalCost)
        {
            isTopPlayed = true;
            playerControler.TopEnergy -= topCost;
            playerControler.BottomEnergy -= additionalCost;
            StartCoroutine(SetPlayed());
        }
        else
        {
            PlayFailed();
        }
    }
    public override void AttemptToPlayBottom()
    {
        if (playerControler.BottomEnergy >= bottomCost && playerControler.TopEnergy >= additionalCost)
        {
            isBottomPlayed = true;
            playerControler.TopEnergy -= additionalCost;
            playerControler.BottomEnergy -= bottomCost;
			StartCoroutine(SetPlayed());
		}
		else
        {
            PlayFailed();
        }
    }
}
