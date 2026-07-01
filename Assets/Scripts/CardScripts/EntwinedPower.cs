public class EntwinedPower : Card
{
    public EntwinedPower() : base(2, 1, 1) { }

    private int additionalCost = 1;
    public override void Awake()
    {
        
        additionalTopDescription = "Cost: " + additionalCost + " bottom energy";
        additionalBottomDescription = "Cost: " + additionalCost + " top energy";

        base.Awake();
    }

    // Update is called once per frame

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Draw(3)));
        currentActions.Add( new Action(() => playerControler.Attack(15)));
        currentActions.Add( new Action(() => playerControler.Skill(15)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Draw(3)));
        currentActions.Add( new Action(() => playerControler.Move(15)));
        currentActions.Add( new Action(() => playerControler.Block(15)));
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
