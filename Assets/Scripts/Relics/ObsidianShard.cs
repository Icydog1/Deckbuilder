using UnityEngine;

public class ObsidianShard : Relic
{
    public override void Awake()
    {
        relicName = "Obsidian Shard";
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
