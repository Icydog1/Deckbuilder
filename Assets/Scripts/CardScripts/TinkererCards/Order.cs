using UnityEngine;

public class Order : Card
{
    public Order() : base(1, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command()));
        currentActions.Add(new Action((currentTarget) => currentTarget.Block(7)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Attack(9)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Command()));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(15,true)));
    }
}