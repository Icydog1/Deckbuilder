using System.Collections;
using UnityEngine;

public class ObsidianShard : Relic
{
    public override void Awake()
    {
        relicName = "Obsidian Shard";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Strength(2, -1)), relicDescriptionList));

        yield return StartCoroutine(base.OnGain());
        if (relicDescriptionList != null && relicDescriptionList.Count > 0)
        {
            Debug.Log("Come Back to this");
            //relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
        }
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Strength(2, -1)), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());
    }
}
