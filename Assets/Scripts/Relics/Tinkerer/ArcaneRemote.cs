using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ArcaneRemote : Relic
{
    protected override int rarity => 2;
    private BasicActionPreformer actionPreformer;
    public override void Awake()
    {
        relicDesription = "At the start of your turn preform command <sprite name=Target>1 summon: <sprite name=Move>" + Var.relicIncreaseableNumberColor + Var.arcaneRemoteMove + "</color>";
        base.Awake();
    }
    //prepares a basic action preformer to do a command move
    public override IEnumerator OnGain()
    {
        actionPreformer = new BasicActionPreformer(new List<Action>()
        {
            new Action(() => playerControler.Command())
            ,new Action((currentTarget) => currentTarget.Move(Var.arcaneRemoteMove * count))
        });
        playerControler.PlayerTurnFirstActions += StartOfTurnCommand;
        yield return StartCoroutine(base.OnGain());


    }
    //updates description
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
        yield return relicManager.StartCoroutine(actionPreformer.PreformActions(true));

    }
    public IEnumerator StartOfTurnCommand(PlayerControler playerControler)
    {
        yield return relicManager.StartCoroutine(actionPreformer.PreformActions());
    }

    public void OnDestroy()
    {
        playerControler.PlayerTurnFirstActions -= StartOfTurnCommand;
    }

}
