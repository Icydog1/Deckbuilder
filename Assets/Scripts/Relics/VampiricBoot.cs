using System.Collections;
using UnityEngine;

public class VampiricBoot : Relic
{
    protected override int rarity => 2;
    private int damageDealt, damageDealtTotal;
    public override void Awake()
    {
        relicDesription = "Every " + Variables.vampiricBootDamage + " attack damage you deal to enemies gain <color=#009f9f>1<color=white> burst for " + Variables.vampiricBootBurstDuration + " turns";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.StartedAttackingEnemyFunc += GetInitalDamage;
        playerControler.DoneAttackingEnemyFunc += GetFinalDamage;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
    }

    public void GetInitalDamage(PlayerControler playerControler)
    {
        damageDealtTotal = OverallStatistics.damageDealt;
        //Debug.Log("GotInitalDamage of " + damageDealtTotal);
    }
    public void GetFinalDamage(PlayerControler playerControler)
    {
        int damageDone = OverallStatistics.damageDealt - damageDealtTotal;
        damageDealt += damageDone;
        //Debug.Log("GotFinalDamage of " + damageDone);
        if (damageDealt >= Variables.vampiricBootDamage)
        {
            int times = damageDealt / Variables.vampiricBootDamage; //integer divition (discards remainder)
            actionManager.QueueAction(playerControler.ApplyCondition(new Burst(times * count, Variables.vampiricBootBurstDuration), "self", 1, 1, false, false));
            damageDealt -= times * Variables.vampiricBootDamage;
        }
    }
    public void OnDestroy()
    {
        playerControler.StartedAttackingEnemyFunc -= GetInitalDamage;
        playerControler.DoneAttackingEnemyFunc -= GetFinalDamage;
    }
}

