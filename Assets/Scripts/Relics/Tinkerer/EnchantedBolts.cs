using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class EnchantedBolts : Relic
{
    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "Your summons have " + Var.relicIncreaseableNumberColor + Var.enchantedBoltsMaxHealth + "</color> extra max health";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.EnchantedBoltsCount++;
        foreach (Figure summon in playerControler.CurrentSummons)
        {
            yield return StartCoroutine(actionManager.PreformAction(summon.GainMaxHealth(Var.enchantedBoltsMaxHealth), relicDescriptionList));
        }
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        playerControler.EnchantedBoltsCount++;
        foreach (Figure summon in playerControler.CurrentSummons)
        {
            yield return StartCoroutine(actionManager.PreformAction(summon.GainMaxHealth(Var.enchantedBoltsMaxHealth), relicDescriptionList));
        }
        yield return StartCoroutine(base.IncreaseCount());

    }
}
