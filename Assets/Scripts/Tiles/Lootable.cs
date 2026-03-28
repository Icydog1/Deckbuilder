using UnityEngine;

public class Lootable : MonoBehaviour
{
    [SerializeField]
    private GameObject lootedTile;
    private RewardManager rewardManager;
    private bool isCard;
    private bool isRelic;
    private float rarity = 1;
    [SerializeField]
    private int lockpickDifficulty = 10;
    public int LockpickDifficulty { get { return lockpickDifficulty; } set { lockpickDifficulty = value; } }
    public float Rarity { get { return rarity; }}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Looted()
    {
        Instantiate(lootedTile,transform.position,transform.rotation);
        Destroy(gameObject);
    }

    public void Lockpick(int lockpickValue)
    {
        LockpickDifficulty -= lockpickValue;
        if (LockpickDifficulty <= 0)
        {
            rewardManager.TileReward(gameObject);
        }
    }
}
