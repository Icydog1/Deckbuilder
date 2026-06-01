using UnityEngine;

public class RefrenceStorage : MonoBehaviour
{
    //private GameManager gameManager;
    //public GameManager GameManager { get { return gameManager; } }
    //private TurnManager turnManager;
    //public TurnManager TurnManager { get { return turnManager; }}
    //private MapManager mapManager;
    //public MapManager MapManager { get { return mapManager; } }
    //private MouseManager mouseManager;
    //public MouseManager MouseManager { get { return mouseManager; } }


    //private PlayerControler playerControler;
    //public PlayerControler PlayerControler { get { return playerControler; } }

    //private Pathfinder pathfinder;
    //public Pathfinder Pathfinder { get { return pathfinder; } }
    //private DeckManager deckManager;
    //public DeckManager DeckManager { get { return deckManager; } }
    //private LevelManager levelManager;
    //public LevelManager LevelManager { get { return levelManager; } }
    //private ActionManager actionManager;
    //public ActionManager ActionManager { get { return actionManager; } }
    //private OverallStatistics overallStatistics;
    //public OverallStatistics OverallStatistics { get { return overallStatistics; } }
    //private RoomSpawner roomSpawner;
    //public RoomSpawner RoomSpawner { get { return roomSpawner; } }
    //private RewardManager rewardManager;
    //public RewardManager RewardManager { get { return rewardManager; } }
    //private ConditionEffects conditionEffects;
    //public ConditionEffects ConditionEffects { get { return conditionEffects; } }


    //private GameObject pauseScreenBlocker;
    //public GameObject PauseScreenBlocker { get { return pauseScreenBlocker; } }

    public static GameManager gameManager;
    public static TurnManager turnManager;
    public static MapManager mapManager;
    public static MouseManager mouseManager;
    public static PlayerControler playerControler;
    public static Pathfinder pathfinder;
    public static DeckManager deckManager;
    public static LevelManager levelManager;
    public static ActionManager actionManager;
    public static OverallStatistics overallStatistics;
    public static RoomSpawner roomSpawner;
    public static RewardManager rewardManager;
    public static ConditionEffects conditionEffects;


    public static GameObject pauseScreenBlocker;

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
        overallStatistics = GameObject.Find("OverallStatistics").GetComponent<OverallStatistics>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        conditionEffects = GameObject.Find("ConditionEffects").GetComponent<ConditionEffects>();



        pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");

    }

}
