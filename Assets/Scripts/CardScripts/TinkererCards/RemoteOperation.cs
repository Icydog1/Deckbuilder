using UnityEngine;

public class RemoteOperation : Card
{
    public RemoteOperation() : base(1, 2, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command()));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(16)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Attack(16,4)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.AddKeyword("Augment")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Speed(2))));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Speed(-1))));
    }
}