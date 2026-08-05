using System;
using System.Collections;
using UnityEngine;

public class AdaptiveShield : Relic
{
    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "Whenever you lose HP gain <color=#009f9f>" + Var.adaptiveShieldBlock + "<color=white> block next turn";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        //playerControler.AdaptiveShieldCount++;

        playerControler.LostHealth += GainBlockOnLosingHP;

        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        //playerControler.AdaptiveShieldCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
    public void GainBlockOnLosingHP(PlayerControler playerControler)
    {
        actionManager.QueueAction(playerControler.ApplyCondition(new StartOfTurnBlock(Var.adaptiveShieldBlock * count)));
    }

    public void OnDestroy()
    {
        playerControler.LostHealth -= GainBlockOnLosingHP;
    }
}

