public class KineticBattery : Relic
{
    public override void Awake()
    {
        relicName = "Kinetic Battery";
        relicDesription = "Whenever you move a space gain 1 Vigor";
        base.Awake();
    }
    public override void OnGain()
    {
        playerControler.KineticBatteryCount++;
        base.OnGain();

    }
    public override void IncreaseCount()
    {
        playerControler.KineticBatteryCount++;
        base.IncreaseCount();
    }
}

