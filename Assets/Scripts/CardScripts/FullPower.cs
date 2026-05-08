using System.Collections;
public class FullPower : Card
{

    public override void Start()
    {
        topCost = 2;
        bottomCost = 2;
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }


    public override IEnumerator PrepareTop()
    {
        yield return null;
        currentActions.Add(() => playerControler.Attack(20));
        currentActions.Add(() => playerControler.Attack(20));

    }

    public override IEnumerator PrepareBottom()
    {
        yield return null;

        currentActions.Add(() => playerControler.Move(35));

    }
}
