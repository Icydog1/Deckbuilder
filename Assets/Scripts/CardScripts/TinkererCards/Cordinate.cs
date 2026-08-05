using UnityEngine;

public class Cordinate : Card
{
    public Cordinate() : base(1, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(13)));
        currentActions.Add(new Action(() => playerControler.Command()));
        currentActions.Add(new Action((currentTarget) => currentTarget.Attack(12)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(13)));
        currentActions.Add(new Action(() => playerControler.Command()));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(12)));
    }
}