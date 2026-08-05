using System.Collections;
using UnityEngine;

 public class LongShot : Card
{
    public LongShot() : base(1, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(13, 6)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Skill(20)));
    }
}
