using System.Collections;
using UnityEngine;

public class StormBoundPearl : Relic
{
    protected override int rarity => 2;

    public override void Awake()
    {
        relicDesription = "When you open a door gain <color=#009f9f>1<color=white> top energy";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.OpenedDoorFunc += GainTopEnergyOnOpeningDoor;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
    }
    public void GainTopEnergyOnOpeningDoor(PlayerControler playerControler)
    {
        actionManager.PrepareAction(playerControler.GainTopEnergy(count));
    }

    public void OnDestroy()
    {
        playerControler.OpenedDoorFunc -= GainTopEnergyOnOpeningDoor;
    }
}