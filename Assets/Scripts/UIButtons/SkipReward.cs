using UnityEngine;

public class SkipReward : UIButton
{
    private RewardManager rewardManager;
    private MouseManager mouseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();

        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Activate()
    {
        //Debug.Log("skiping reward");
        StartCoroutine(rewardManager.RewardScrapped());
        mouseManager.MouseOffObject(gameObject);
    }

}