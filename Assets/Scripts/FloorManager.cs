using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    private RewardManager rewardManager;
    private MapManager mapManager;
    private RoomSpawner roomSpawner;
    private Camera camera;
    private GameObject player;
    private PlayerControler playerControler;
    private TurnManager turnManager;

    private bool isBossFloor;
    //private int level;
    private int difficulty;
    private int floorRoundNumber;

    [SerializeField]
    private GameObject[] bossRooms;

    [SerializeField]
    private GameObject stair;

    public static event Action<FloorManager> FloorClearedFuntions, FloorGeneratedFuntions;
    public static event Func<FloorManager, IEnumerator> FloorGenerated;
    public static event Func<FloorManager, IEnumerator> FloorCleared;

    private List<GameObject> floorSpecific = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

    }

    void Start()
    {
        rewardManager = RefrenceStorage.rewardManager;
        mapManager = RefrenceStorage.mapManager;
        roomSpawner = RefrenceStorage.roomSpawner;
        player = RefrenceStorage.player;
        camera = RefrenceStorage.camera;
        playerControler = RefrenceStorage.playerControler;
        turnManager = RefrenceStorage.turnManager;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartFloor()
    {
        OverallStatistics.floor = 1;
        isBossFloor = false;
        //player.transform.position = new Vector3(0, 0, player.transform.position.z);
        camera.transform.position = new Vector3(0, 0, camera.transform.position.z);
        //playerControler.OneToOnePos = Vector2.zero;
        roomSpawner.SpawnStartingRoom();
        //Debug.Log("Started Level Room");

    }

    public IEnumerator GoUpFloor()
    {
        yield return StartCoroutine(ClearFloor());
        yield return StartCoroutine(playerControler.GoUpFloor());
        camera.transform.position = new Vector3(0, 0, camera.transform.position.z);
        OverallStatistics.floor++;
        if (!isBossFloor)
        {
            isBossFloor = true;
            //Debug.Log("went up level");
            floorSpecific.Add(Instantiate(bossRooms[UnityEngine.Random.Range(0, bossRooms.Length)]));
        }
        else
        {
            isBossFloor = false;
            roomSpawner.SpawnStartingRoom();
            RefrenceStorage.interactButton.SetActive(true);
        }
        StartCoroutine(turnManager.StartTakingTurns());
    }

    public void IncreaseRoundNumber()
    {
        difficulty++;
        floorRoundNumber++;
    }
    //public IEnumerator GetDifficultyModifier(Enemy enemy)
    //{
    //    //yield return StartCoroutine(RefrenceStorage.actionManager.PreformAction(enemy.ApplyCondition(new NaturalScaling(difficulty))));
    //}
    public IEnumerator ResetGame()
    {
        difficulty = 0;
        yield return StartCoroutine(ClearFloor());
        //Debug.Log("ResetGame");
    }

    public IEnumerator ClearFloor()
    {
        //Debug.Log("Clear Level 1");
        
        if (FloorClearedFuntions != null)
        {
            FloorClearedFuntions(this);
        }
        if (FloorCleared != null)
        {
            Delegate[] listeners = FloorCleared.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a FloorManager and outputs a IEnumerator
                var callback = (Func<FloorManager, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
        }
        foreach (GameObject gameObject in floorSpecific)
        {
            Destroy(gameObject);
        }
        //Debug.Log("Clear Level 2");

        if (FloorGeneratedFuntions != null)
        {
            FloorGeneratedFuntions(this);
        }
        //Debug.Log("Clear Level 3");
        if (FloorGenerated != null)
        {
            Delegate[] listeners = FloorGenerated.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a FloorManager and outputs a IEnumerator
                var callback = (Func<FloorManager, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
        }
        //Debug.Log("Clear Level 4");

    }

    public void BossKilled(Vector2 bossCords)
    {
        OverallStatistics.bossDifficulty++;
        rewardManager.BossReward();
        playerControler.HealDamage(playerControler.MaxHealth);
        Destroy(mapManager.GetTileAtHex(bossCords));
        floorSpecific.Add(Instantiate(stair, mapManager.OneToOneToPos(bossCords), Quaternion.identity));

    }
}
