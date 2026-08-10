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
        currentActions.Add(new Action(() => playerControler.Command("enemy", 4, 1)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(24)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Attack(27, targetType: "friendly")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Stunned())));
        currentActions.Add(new Action(() => playerControler.Exhausting(4)));


    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Command("enemy", 3, Var.infinityValue)));
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