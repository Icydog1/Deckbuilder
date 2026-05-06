using System.Collections.Generic;

public class BetterLockpicks : Relic
{
    public BetterLockpicks()
    {
        isUnique = true;
    }
    public override void Awake()
    {
        relicName = "Better Lockpicks";
        base.Awake();
    }
    public override void OnGain()
    {
        playerControler.GainAbility(new Ability(1, new List<System.Action>() { () => playerControler.Lockpick(2, true) }));

        base.OnGain();

    }
}
