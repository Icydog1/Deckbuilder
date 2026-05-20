using UnityEngine;

public class RefrenceStorage : MonoBehaviour
{
    private GameManager gameManager;
    public GameManager GameManager { get { return gameManager; } }
    private TurnManager turnManager;
    public TurnManager TurnManager { get { return turnManager; }}
    protected MapManager mapManager;
    public MapManager MapManager { get { return mapManager; } }
    protected MouseManager mouseManager;
    public MouseManager MouseManager { get { return mouseManager; } }

    //protected FigureStats statsDisplayer;
    //public FigureStats FigureStats { get { return mapManager; } }

    protected PlayerControler playerControler;
    public PlayerControler PlayerControler { get { return playerControler; } }

    protected Pathfinder pathfinder;
    public Pathfinder Pathfinder { get { return pathfinder; } }
    //protected ConditionEffects conditionEffects;
    //public ConditionEffects ConditionEffects { get { return mapManager; } }
    protected DeckManager deckManager;
    public DeckManager DeckManager { get { return deckManager; } }
    protected LevelManager levelManager;
    public LevelManager LevelManager { get { return levelManager; } }
    protected ActionManager actionManager;
    public ActionManager ActionManager { get { return actionManager; } }
    protected OverallStatistics overallStatistics;
    public OverallStatistics OverallStatistics { get { return overallStatistics; } }
    protected RoomSpawner roomSpawner;
    public RoomSpawner RoomSpawner { get { return roomSpawner; } }
    protected RewardManager rewardManager;
    public RewardManager RewardManager { get { return rewardManager; } }
    

    private GameObject pauseScreenBlocker;
    public GameObject PauseScreenBlocker { get { return pauseScreenBlocker; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        //conditionEffects = GameObject.Find("ConditionEffects").GetComponent<ConditionEffects>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
        overallStatistics = GameObject.Find("OverallStatistics").GetComponent<OverallStatistics>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();



        pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");

    }

}
