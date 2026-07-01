using UnityEngine;

public class Plating : Card
{
    public Plating() : base(2, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command()));
        currentActions.Add(new Action((currentTarget) => currentTarget.Block(20)));
        //currentActions.Add(new Action((currentTarget) => playerControler.GainTopEnergy(3)));
        //currentActions.Add(new Action((currentTarget) => playerControler.GainBottomEnergy(3)));
    }

    public override void PrepareBottom()
    {

        currentActions.Add(new Action(() => playerControler.AddKeyword("Augment")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new StartOfTurnBlock(4, Variables.gameInfinityValue))));
        //discard card also
    }
}