using System;
using System.Collections;
using UnityEngine;

public class AdaptiveShield : Relic
{
    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "Whenever you lose HP gain <color=#009f9f>" + Variables.adaptiveShieldBlock + "<color=white> block next turn";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        //playerControler.AdaptiveShieldCount++;

        playerControler.LostHealth += GainBlockOnLosingHP;

        yield return StartCoroutine(base.OnGain());
        //if (relicDescriptionList != null && relicDescriptionList.Count > 0)
        //{
        //    Debug.Log("Come Back to this");
        //    //relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
        //}
    }
    public override IEnumerator IncreaseCount()
    {
        //playerControler.AdaptiveShieldCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
    public void GainBlockOnLosingHP(PlayerControler playerControler)
    {
        actionManager.QueueAction(playerControler.ApplyCondition(new NextTurnBlock(Variables.adaptiveShieldBlock * count)));
    }

    public void OnDestroy()
    {
        playerControler.LostHealth -= GainBlockOnLosingHP;
    }
}

