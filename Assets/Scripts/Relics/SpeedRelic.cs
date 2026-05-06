using UnityEngine;

public class SpeedRelic : Relic
{
    public override void Awake()
    {
        relicName = "SpeedRelic";
        base.Awake();
    }
    public override void OnGain()
    {
        playerControler.ApplyCondition(new Speed(2, -1));
        base.OnGain();

    }
    public override void IncreaseCount()
    {
        playerControler.ApplyCondition(new Speed(2, -1));
        base.IncreaseCount();
    }
}
