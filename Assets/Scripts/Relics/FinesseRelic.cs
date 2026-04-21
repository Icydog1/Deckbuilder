using UnityEngine;

public class FinesseRelic : Relic
{
    public override void Awake()
    {
        relicName = "finesseRelic";
        base.Awake();
    }
    public override void OnGain()
    {
        playerControler.ApplyCondition(new Finesse(1, -1));
        base.OnGain();

    }
    public override void IncreaseCount()
    {
        playerControler.ApplyCondition(new Finesse(1, -1));
        base.IncreaseCount();
    }
}
