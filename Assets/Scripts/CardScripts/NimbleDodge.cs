using System.Collections;
using UnityEngine;

public class NimbleDodge : Card
{
    public NimbleDodge() : base(1, 1, 1) { }

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
        currentActions.Add( new Action(() => playerControler.Block(25)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(15, true)));
    }
}
