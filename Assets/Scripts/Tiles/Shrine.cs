using System.Collections;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    private PlayerControler playerControler;
    private RewardManager rewardManager;
    private Interactable interactable;


    void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();

        interactable = gameObject.GetComponent<Interactable>();

        interactable.InteractedWith.AddListener(GainLevelFunction);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GainLevelFunction()
    {
        //Debug.Log("Gain Level Function");

        StartCoroutine(GainLevels());
    }
    public IEnumerator GainLevels()
    {
        //Debug.Log("Gain Level coroutine");

        while (playerControler.PotentialLevel > playerControler.Level)
        {
            //Debug.Log("Gained Level");

            yield return StartCoroutine(playerControler.LevelUp());
        }
    }
}
