using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class HalfFilledPotion : Relic
{

    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "Heal " + Var.relicIncreaseableNumberColor + Var.halfFilledPotionHealingPercent + "%</color> of your max health";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.Heal(Var.halfFilledPotionHealingPercent * playerControler.MaxHealth / 100), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.Heal(Var.halfFilledPotionHealingPercent * playerControler.MaxHealth / 100), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());
    }
}
