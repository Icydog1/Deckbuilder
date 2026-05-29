using System.Collections;
using UnityEngine;

public class KillQuest : Quest
{
    [SerializeField]
    int killAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        description = "Kill " + killAmount + " enemies";
        base.Awake();
    }


    public override IEnumerator GainQuest()
    {
        isActive = true;
        //int killAmountRequred = overallStatistics.TotalEnemiesKilled + killAmount;
        yield return StartCoroutine(base.GainQuest());
        for (int killsLeft = killAmount; killsLeft > 0; killsLeft--)
        {
            SetDescription("Kill " + killsLeft + " enemies");
            int WaitUntilValue = OverallStatistics.totalEnemiesKilled + 1;
            yield return new WaitUntil(() => OverallStatistics.totalEnemiesKilled >= WaitUntilValue);
        }
        Debug.Log("quest sucsesfull");
        CompleteQuest();

    }
    public override IEnumerator Reward()
    {
        yield return StartCoroutine(base.Reward());
        yield break;
    }

}
