using System.Collections;

public class SkillfulStab : Card
{
    public override void Start()
    {
        topCost = 1;
        bottomCost = 0;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override IEnumerator PrepareTop()
    {
        yield return null;

        currentActions.Add(() => playerControler.Attack(10));
        currentActions.Add(() => playerControler.Draw(1));

    }

    public override IEnumerator PrepareBottom()
    {
        yield return null;

        currentActions.Add(() => playerControler.Ability(10));
    }
}
