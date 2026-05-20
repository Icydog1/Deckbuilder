using System.Collections;
using UnityEngine;

public class KillQuest : Quest
{
    [SerializeField]
    int killAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        base.Awake();
    }


    public override IEnumerator GainQuest()
    {
        isActive = true;
        int killAmountRequred = overallStatistics.TotalEnemiesKilled + killAmount;
        yield return new WaitUntil(() => overallStatistics.TotalEnemiesKilled >= killAmountRequred);
        Debug.Log("quest sucsesfull");
        CompleteQuest();

    }
    public override IEnumerator Reward()
    {
        yield return StartCoroutine(base.Reward());
        yield break;
    }

}
