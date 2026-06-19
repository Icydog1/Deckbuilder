using UnityEngine;

public class RefrenceStorage : MonoBehaviour
{
    public static GameManager gameManager;
    public static TurnManager turnManager;
    public static MapManager mapManager;
    public static MouseManager mouseManager;
    public static PlayerControler playerControler;
    public static Pathfinder pathfinder;
    public static DeckManager deckManager;
    public static LevelManager levelManager;
    public static ActionManager actionManager;
    //public static OverallStatistics overallStatistics;
    public static RoomSpawner roomSpawner;
    public static RewardManager rewardManager;
    public static ConditionEffects conditionEffects;
    public static PlayerStats playerStats;
    public static CompendiumManager compendiumManager;
    public static CameraScript cameraScript;

    public static GameObject interactButton;
    public static GameObject pauseScreenBlocker;
    public static GameObject deathScreenBlocker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
        //overallStatistics = GameObject.Find("OverallStatistics").GetComponent<OverallStatistics>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        conditionEffects = GameObject.Find("ConditionEffects").GetComponent<ConditionEffects>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
        compendiumManager = GameObject.Find("CompendiumScreenBlocker").GetComponent<CompendiumManager>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();

        interactButton = GameObject.Find("InteractButton");

        pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");
        deathScreenBlocker = GameObject.Find("DeathScreenBlocker");

    }

}
