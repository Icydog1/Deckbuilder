using System.Collections;

public class ShieldedStrike : Card
{
    public override void Start()
    {
        topCost = 1;
        bottomCost = 1;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override IEnumerator PrepareTop()
    {
        yield return "Temp";

        currentActions.Add(() => playerControler.Block(10));
        currentActions.Add(() => playerControler.Attack(10));
    }

    public override IEnumerator PrepareBottom()
    {
        yield return "Temp";
        currentActions.Add(() => playerControler.Ability(10));
        //currentActions.Add(() => playerControler.ApplyCondition(new GainAbility(new Ability(1, new List<System.Action>() { () => playerControler.Block(1, true) }))));
        
    }
}
