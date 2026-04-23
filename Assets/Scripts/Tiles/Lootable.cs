using UnityEngine;

public class Lootable : MonoBehaviour
{
    [SerializeField]
    private GameObject lootedTile;
    private RewardManager rewardManager;
    private VariableDisplayer lockpickRemainingDisplay;
    [SerializeField]
    private bool isCard;
    private float rarity = 1;
    [SerializeField]
    private int lockpickDifficulty = 10;
    public int LockpickDifficulty { get { return lockpickDifficulty; } set { lockpickDifficulty = value; lockpickRemainingDisplay.DisplayText(lockpickDifficulty);}}
    public float Rarity { get { return rarity; }}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        lockpickRemainingDisplay = transform.Find("TileUI").Find("LockpickRemaningText").GetComponent<VariableDisplayer>();
    }
    void Start()
    {

        lockpickRemainingDisplay.DisplayText(lockpickDifficulty);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Looted()
    {
        GameObject newTile = Instantiate(lootedTile,transform.position,transform.rotation);
        newTile.GetComponent<Tile>().MoveCost = GetComponent<Tile>().MoveCost;
        Destroy(gameObject);
    }

    public void Lockpick(int lockpickValue)
    {
        LockpickDifficulty -= lockpickValue;
        if (LockpickDifficulty <= 0)
        {
            rewardManager.TileReward(gameObject, isCard);
        }
    }
}
