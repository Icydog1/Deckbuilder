using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Recharge : WizardCard
{
    public Recharge() : base(1, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.GainMana(10)));
        currentActions.Add(new Action(() => playerControler.Draw(1)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => RefillMana(), "Gain mana until you are at capacity"));
    }

    public IEnumerator RefillMana()
    {
        actionManager.ActionStackNames.Push("RefillMana");
        playerControler.Mana = Mathf.Max(playerControler.Mana, playerControler.ManaCapacity);
        playerControler.EndAction();
        yield break;
    }
}