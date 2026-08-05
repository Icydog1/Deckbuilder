using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Unleash : WizardCard
{
    public Unleash() : base(2, 0, 1, 0, 16) {}

    public override void OtherDescriptionPreperation()
    {
        topCostText.DisplayString("<color=red>" + topCost + " <color=#D000D0>(X)");
    }
    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => AttackForMana()));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Vigor(12, 1))));
        currentActions.Add(new Action(() => playerControler.GainTopEnergy(2)));
    }
    public IEnumerator AttackForMana()
    {
        //Attack 7 for each card discarded
        if (playerControler.IsPlanning)
        {
            //just needs to not be 1
            yield return StartCoroutine(playerControler.Attack(Var.changeingValue));
            ActionDescription plan = actionManager.PlanToList[actionManager.PlanToList.Count - 1];
            for (int i = 0; i < plan.ActionModifiers.Count; i++)
            {
                if (plan.ActionModifiers[i].Type == "Attack")
                {
                    plan.ActionModifiers[i] = new ActionModifier(playerControler, " <sprite name=Attack>2x", valueType: "Repeats");
                    break;
                }
            }
        }
        else
        {
            int manaSpent = playerControler.Mana;
            playerControler.Mana = 0;
            yield return StartCoroutine(playerControler.Attack(2 * manaSpent));
        }


        //actionManager.ActionStackNames.Push("DoublePoison");
        //playerControler.EndAction();

    }
}