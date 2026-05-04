using UnityEngine;

public class DexterityRelic : Relic
{
    public override void Awake()
    {
        relicName = "dexterityRelic";
        base.Awake();
    }
    public override void OnGain()
    {
        playerControler.ApplyCondition(new Dexterity(2, -1));
        base.OnGain();

    }
    public override void IncreaseCount()
    {
        playerControler.ApplyCondition(new Dexterity(2, -1));
        base.IncreaseCount();
    }
}
