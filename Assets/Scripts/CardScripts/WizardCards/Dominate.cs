using System.Collections;
using UnityEngine;

 public class Dominate : WizardCard
{
    public Dominate() : base(3, 2, 1) { }

    public override void Awake()
    {

        additionalTopDescription = "Cost: 1 bottom energy";

        base.Awake();
    }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command(1, "enemy", 4)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(24)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Attack(27, targetType: "friendly")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Stunned())));
        currentActions.Add(new Action(() => playerControler.Exhausting(4)));


    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Command(Var.infinityValue, "enemy", 3)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Attack(12, targetType: "friendly")));
    }

    public override void AttemptToPlayTop()
    {
        if (playerControler.TopEnergy >= topCost && playerControler.BottomEnergy >= 2)
        {
            isTopPlayed = true;
            playerControler.TopEnergy -= topCost;
            playerControler.BottomEnergy -= 2;
            StartCoroutine(SetPlayed());
        }
        else
        {
            PlayFailed();
        }
    }
}