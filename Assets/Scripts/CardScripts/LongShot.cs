using System.Collections;
using UnityEngine;

 public class LongShot : Card
{
    public LongShot() : base(1, 1, 1) { }

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
        currentActions.Add( new Action(() => playerControler.Attack(15, 6)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Ability(20)));
    }
}
