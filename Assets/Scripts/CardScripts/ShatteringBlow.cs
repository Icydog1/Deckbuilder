using System.Collections;

public class ShatteringBlow : Card
{
    public override void Start()
    {
        topCost = 2;
        bottomCost = 1;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override IEnumerator PrepareTop()
    {
        yield return null;

        currentActions.Add(() => playerControler.Attack(20, 1, 1, 1, new Condition[] { new Strength(-10, 1) } ));
    }

    public override IEnumerator PrepareBottom()
    {
        yield return null;

        currentActions.Add(() => playerControler.Move(15));
        currentActions.Add(() => playerControler.ApplyCondition(new Strength(10, 1)));

    }
}
