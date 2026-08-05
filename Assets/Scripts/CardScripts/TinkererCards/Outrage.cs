using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Outrage : Card
{
    public Outrage() : base(2, 1, 1) { }
    private int cardsInHand;

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => GetCardsInHand()));
        currentActions.Add(new Action(() => playerControler.Discard(Var.infinityValue)));
        currentActions.Add(new Action(() => AttackForEach(6)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Draw(3)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Strength(-4,1))));

    }
    public IEnumerator GetCardsInHand()
    {
        if (playerControler.IsPlanning)
        {

        }
        else
        {
            actionManager.ActionStackNames.Push("GetCardsInHand");
            cardsInHand = deckManager.handContents.Count;
            playerControler.EndAction();
            yield break;
        }
    }
    public IEnumerator AttackForEach(int attackDamage)
    {
        //Attack 7 for each card discarded
        if (playerControler.IsPlanning)
        {
            //just needs to not be 1
            yield return StartCoroutine(playerControler.Attack(attackDamage, repeats: Var.changeingValue));
            ActionDescription plan = actionManager.PlanToList[actionManager.PlanToList.Count - 1];
            for (int i = 0; i < plan.ActionModifiers.Count; i++)
            {
                if (plan.ActionModifiers[i].Type == "Repeats")
                {
                    plan.ActionModifiers[i] = new ActionModifier(playerControler, " x times, where x is the number of cards discarded", valueType: "Repeats");
                    break;
                }
            }
        }
        else
        {
            Debug.Log(cardsInHand + " times");
            yield return StartCoroutine(playerControler.Attack(attackDamage, repeats: cardsInHand));
        }


        //actionManager.ActionStackNames.Push("DoublePoison");
        //playerControler.EndAction();

    }
}