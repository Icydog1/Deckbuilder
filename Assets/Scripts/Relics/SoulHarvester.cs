using System.Collections;
using UnityEngine;

public class SoulHarvester : Relic
{
    protected override int rarity => 3;

    public override void Awake()
    {
        relicDesription = "When you kill an enemy gain <color=#009f9f>1<color=white> health";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.KilledEnemyFunc += HealOnkill;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
    }
    public void HealOnkill(PlayerControler playerControler)
    {
        actionManager.QueueAction(playerControler.Heal(count));
    }

    public void OnDestroy()
    {
        playerControler.KilledEnemyFunc -= HealOnkill;
    }
}