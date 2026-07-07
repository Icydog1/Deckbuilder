using System.Collections;
using UnityEngine;

public class ExploreQuest : Quest
{
    [SerializeField]
    int numberOfRequredRooms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        description = "Explore " + numberOfRequredRooms + " rooms";
        base.Awake();
    }


    public override IEnumerator GainQuest()
    {
        isActive = true;
        //int killAmountRequred = overallStatistics.TotalEnemiesKilled + killAmount;
        yield return StartCoroutine(base.GainQuest());
        for (int roomsLeft = numberOfRequredRooms; roomsLeft > 0; roomsLeft--)
        {
            SetDescription("Explore " + roomsLeft + " rooms");
            int WaitUntilValue = OverallStatistics.roomsExplored + 1;
            yield return new WaitUntil(() => OverallStatistics.roomsExplored >= WaitUntilValue);
        }
        //Debug.Log("quest sucsesfull");
        CompleteQuest();

    }
    public override IEnumerator Reward()
    {
        yield return StartCoroutine(base.Reward());
        yield break;
    }

}
