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
    public static FloorManager floorManager;
    public static ActionManager actionManager;
    public static RoomSpawner roomSpawner;
    public static RewardManager rewardManager;
    public static ConditionEffects conditionEffects;
    public static PlayerStats playerStats;
    public static CompendiumManager compendiumManager;
    public static CameraScript cameraScript;
    public static Camera camera;
    public static RelicManager relicManager;
    public static UIManager UIManager;
    public static MainMenuManager mainMenuManager;
    public static TooltipManager tooltipManager;
    public static AbilityManager abilityManager;
    

    public static GameObject player;
    public static GameObject interactButton;
    public static GameObject pauseScreenBlocker, deathScreenBlocker, rewardScreenBlocker, listDisplayerScreenBlocker, compendiumScreenBlocker, mainMenuScreenBlocker, characterSelectScreenBlocker;
    public static GameObject UI;
    public static GameObject tooltip;
    public static GameObject mainMenu;

    //Stores refrences to scripts and gameObjects for easy retrival
    void Awake()
    {


        player = GameObject.Find("Player");

        interactButton = GameObject.Find("InteractButton");
        pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");
        deathScreenBlocker = GameObject.Find("DeathScreenBlocker");
        rewardScreenBlocker = GameObject.Find("RewardScreenBlocker");
        listDisplayerScreenBlocker = GameObject.Find("ListDisplayerScreenBlocker");
        compendiumScreenBlocker = GameObject.Find("CompendiumScreenBlocker");
        mainMenuScreenBlocker = GameObject.Find("MainMenuScreenBlocker");
        characterSelectScreenBlocker = GameObject.Find("CharacterSelectScreenBlocker");

        UI = GameObject.Find("UI");
        tooltip = GameObject.Find("Tooltip");
        mainMenu = mainMenuScreenBlocker.transform.Find("MainMenu").gameObject;


        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        playerControler = player.GetComponent<PlayerControler>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
        //overallStatistics = GameObject.Find("OverallStatistics").GetComponent<OverallStatistics>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        conditionEffects = GameObject.Find("ConditionEffects").GetComponent<ConditionEffects>();
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
        compendiumManager = GameObject.Find("CompendiumScreenBlocker").GetComponent<CompendiumManager>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        tooltipManager = GameObject.Find("Tooltip").GetComponent<TooltipManager>();

        relicManager = GameObject.Find("RelicManager").GetComponent<RelicManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        mainMenuManager = GameObject.Find("MainMenuManager").GetComponent<MainMenuManager>();
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();

    }

}
