using System;
using System.Collections;
using System.Collections.Generic;

public class Train : Card
{
    public Train() : base(1, 1, 1){ }

    public override void PrepareTop()
    {
        updateDescriptionKeywords.Add("AttackValue");
        currentActions.Add(new Action(() => SkillIncreasedByStrength(10)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(2, new List<Action>() { new Action(() => playerControler.ApplyCondition(new Strength(1, 1), isVariable: true)) }), 2))));
    }
    public IEnumerator SkillIncreasedByStrength(int skill)
    {
        int strengthIncrease = playerControler.GetValueOfCondition("Strength");
        if (playerControler.IsPlanning)
        {
            yield return StartCoroutine(playerControler.Skill(Var.changeingValue));
            ActionDescription plan = actionManager.PlanToList[actionManager.PlanToList.Count - 1];
            for (int i = 0; i < plan.ActionModifiers.Count; i++)
            {
                if (plan.ActionModifiers[i].Type == "Skill")
                {
                    if (playerControler.UnmodifiedAction)
                    {
                        plan.ActionModifiers[i] = new ActionModifier(playerControler, "<sprite name=Skill>", skill, " this is affected by strength", valueType: "Skill");
                    }
                    else
                    {
                        plan.ActionModifiers[i] = new ActionModifier(playerControler, "<sprite name=Skill>", skill + strengthIncrease, " this is affected by strength", valueType: "Skill");
                    }
                    break;
                }
            }
        }
        else
        {
            yield return StartCoroutine(playerControler.Skill(skill + strengthIncrease));
        }


        //actionManager.ActionStackNames.Push("DoublePoison");
        //playerControler.EndAction();

    }
}