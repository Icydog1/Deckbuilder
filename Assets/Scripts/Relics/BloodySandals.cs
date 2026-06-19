using System.Collections;
using UnityEngine;

public class BloodySandals : Relic
{
    protected override int rarity => 3;
    private int damageDealt, damageDealtTotal;
    public override void Awake()
    {
        relicDesription = "Every " + Variables.bloodySandalsDamage + " attack damage you deal to enemies gain <color=#009f9f>1<color=white> burst for " + Variables.bloodySandalsBurstDuration + " turns";
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
        if (damageDealt >= Variables.bloodySandalsDamage)
        {
            int times = damageDealt / Variables.bloodySandalsDamage; //integer divition (discards remainder)
            actionManager.QueueAction(playerControler.ApplyCondition(new Burst(times * count, Variables.bloodySandalsBurstDuration), "self", 1, 1, false, false));
            damageDealt -= times * Variables.bloodySandalsDamage;
        }
    }
    public void OnDestroy()
    {
        playerControler.StartedAttackingEnemyFunc -= GetInitalDamage;
        playerControler.DoneAttackingEnemyFunc -= GetFinalDamage;
    }
}

