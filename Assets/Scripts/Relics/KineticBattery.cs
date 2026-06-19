using System.Collections;
using UnityEngine;

public class KineticBattery : Relic
{
    protected override int rarity => 3;
    private int steps;
    private bool isGainingVigor;
    public override void Awake()
    {
        relicDesription = "Every " + Variables.kineticBatterySpaces + " spaces you move gain <color=#009f9f>1<color=white> Vigor for " + Variables.kineticBatteryVigorDuration + " turns";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.MovedSpaceFunc += IncreaseSteps;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
    }
    public void IncreaseSteps(PlayerControler playerControler)
    {
        steps++;
        if (!isGainingVigor && steps >= Variables.kineticBatterySpaces)
        {
            actionManager.PrepareAction(GainVigor());
            isGainingVigor = true;
        }
    }

    public IEnumerator GainVigor()
    {
        int vigor = steps / Variables.kineticBatterySpaces;
        yield return StartCoroutine(playerControler.ApplyCondition(new Vigor(vigor * count, Variables.kineticBatteryVigorDuration), "self", 1, 1, false, false));
        steps = 0;
        isGainingVigor = false;
    }

    public void OnDestroy()
    {
        playerControler.MovedSpaceFunc -= IncreaseSteps;
    }
}

