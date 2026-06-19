using System.Collections;
using UnityEngine;

public class CrackedOpal : Relic
{
    protected override int rarity => 2;

    public override void Awake()
    {
        relicDesription = "When you kill an enemy gain <color=#009f9f>1<color=white> bottom energy";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.KilledEnemyFunc += GainBottomEnergyOnkill;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
    }
    public void GainBottomEnergyOnkill(PlayerControler playerControler)
    {
        actionManager.QueueAction(playerControler.GainBottomEnergy(count));
    }

    public void OnDestroy()
    {
        playerControler.KilledEnemyFunc -= GainBottomEnergyOnkill;
    }
}

