using UnityEngine;

public class StrengthRelic : Relic
{
    public override void Awake()
    {
        relicName = "permanentStrengthRelic";
        base.Awake();
    }
    public override void OnGain()
    {
        playerControler.ApplyCondition(new Strength(2, -1));
        base.OnGain();

    }
    public override void IncreaseCount()
    {
        playerControler.ApplyCondition(new Strength(2, -1));
        base.IncreaseCount();
    }
}
