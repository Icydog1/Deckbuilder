using System;
using System.Collections;
using System.Collections.Generic;

public class NumerousPossibilities : Card
{
    public NumerousPossibilities() : base(2, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(1, new List<Action>() { new Action(() => playerControler.Block(1, isVariable: true)) }), 2))));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(2, new List<Action>() { new Action(() => playerControler.Attack(1,3, isVariable: true)) }), 2))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => SkillPerAbility(4)));
    }
    public IEnumerator SkillPerAbility(int skill)
    {
        if (playerControler.IsPlanning)
        {
            yield return StartCoroutine(playerControler.Skill(Var.changeingValue));
            ActionDescription plan = actionManager.PlanToList[actionManager.PlanToList.Count - 1];
            for (int i = 0; i < plan.ActionModifiers.Count; i++)
            {
                if (plan.ActionModifiers[i].Type == "Skill")
                {
                    plan.ActionModifiers[i] = new ActionModifier(playerControler, " <sprite name=Skill>" + skill + "x, where x is the number of abilities you have", valueType: "Skill");
                    break;
                }
            }
        }
        else
        {
            int abilityCount = RefrenceStorage.abilityManager.Abilities.Count;
            yield return StartCoroutine(playerControler.Skill(skill * abilityCount));
        }


        //actionManager.ActionStackNames.Push("DoublePoison");
        //playerControler.EndAction();

    }
}