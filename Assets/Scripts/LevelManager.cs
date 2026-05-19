using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private RewardManager rewardManager;
    private MapManager mapManager;
    private RoomSpawner roomSpawner;
    private Camera camera;
    private GameObject player;
    private PlayerControler playerControler;
    private TurnManager turnManager;

    private bool isBossLevel;
    private int level;
    private int roundNumber;
    private int levelRoundNumber;

    [SerializeField]
    private GameObject[] bossRooms;

    [SerializeField]
    private GameObject stair;

    public static event Action<LevelManager> LevelClearedFuntions, LevelGeneratedFuntions;
    public static event Func<LevelManager, IEnumerator> LevelGenerated;
    public static event Func<LevelManager, IEnumerator> LevelCleared;

    private List<GameObject> levelSpecific = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

    }

    void Start()
    {
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        player = GameObject.Find("Player");
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerControler = player.GetComponent<PlayerControler>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartLevel()
    {
        level = 1;
        isBossLevel = false;
        //player.transform.position = new Vector3(0, 0, player.transform.position.z);
        camera.transform.position = new Vector3(0, 0, camera.transform.position.z);
        //playerControler.OneToOnePos = Vector2.zero;
        roomSpawner.SpawnStartingRoom();
        //Debug.Log("Started Level Room");

    }

    public IEnumerator GoUpLevel()
    {
        yield return StartCoroutine(ClearLevel());
        yield return StartCoroutine(playerControler.GoUpLevel());
        camera.transform.position = new Vector3(0, 0, camera.transform.position.z);
        if (!isBossLevel)
        {
            isBossLevel = true;
            //Debug.Log("went up level");
            levelSpecific.Add(Instantiate(bossRooms[UnityEngine.Random.Range(0, bossRooms.Length)]));
        }
        else
        {
            level++;
            isBossLevel = false;
            roomSpawner.SpawnStartingRoom();
        }
        StartCoroutine(turnManager.StartTakingTurns());
    }

    public void IncreaseRoundNumber()
    {
        roundNumber++;
        levelRoundNumber++;
    }
    public void GetDifficultyModifier(Enemy enemy)
    {
        enemy.ApplyCondition(new NaturalScaling(roundNumber));
    }
    public IEnumerator ResetGame()
    {
        roundNumber = 0;
        yield return StartCoroutine(ClearLevel());
        //Debug.Log("ResetGame");
    }

    public IEnumerator ClearLevel()
    {
        //Debug.Log("Clear Level 1");
        
        if (LevelClearedFuntions != null)
        {
            LevelClearedFuntions(this);
        }
        if (LevelCleared != null)
        {
            yield return StartCoroutine(LevelCleared(this));
        }
        foreach (GameObject gameObject in levelSpecific)
        {
            Destroy(gameObject);
        }
        //Debug.Log("Clear Level 2");

        if (LevelGeneratedFuntions != null)
        {
            LevelGeneratedFuntions(this);
        }
        //Debug.Log("Clear Level 3");

        if (LevelGenerated != null)
        {
            yield return StartCoroutine(LevelGenerated(this));
        }
        //Debug.Log("Clear Level 4");

    }

    public void BossKilled(Vector2 bossCords)
    {
        rewardManager.BossReward();
        playerControler.Heal(playerControler.MaxHealth);
        Destroy(mapManager.GetTileAtHex(bossCords));
        levelSpecific.Add(Instantiate(stair, mapManager.OneToOneToPos(bossCords), Quaternion.identity));

    }
}
