using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemy = new List<GameObject>();
    private int tunsTillActive;
    private TurnManager turnManager;
    private MapManager mapManager;
    private Vector2 OneToOnePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        OneToOnePos = mapManager.PosToOneToOne(transform.position);
        //transform.position = new Vector3(transform.position.x, transform.position.y, spawnHeight);

        SpawnEnemy();

        if (enemy[0].name == "BaseEnemy")
        {
            Debug.Log("Warning: " + gameObject + " tried to summonBase enemy");
        }

        TurnManager.RoundEndedFunctions += AttemptToSpawnEnemy;
        FloorManager.FloorClearedFuntions += Remove;
        tunsTillActive = Var.spawnerActivationDelay;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AttemptToSpawnEnemy(TurnManager turnManager)
    {
        tunsTillActive--;
        if (tunsTillActive <= 0 && Random.Range(0, 1f) <= Var.spawnerSpawnChance)
        {
            if (mapManager.GetEntityOnHex(OneToOnePos) == null)
            {
                SpawnEnemy();
                tunsTillActive = Var.spawnerActivationDelay;
            }
            else
            {
                Debug.Log("spawn obstructed");
            }
        }
        //yield return null;
        //Debug.Log("attempted to spawn");
    }

    public void SpawnEnemy()
    {
        Instantiate(enemy[Random.Range(0,enemy.Count)], new Vector3(transform.position.x, transform.position.y, Var.enemySpawnYElevation), Quaternion.identity);
        //yield return null;
    }
    public void OnDestroy()
    {
        //TurnManager.RoundEnded -= AttemptToSpawnEnemy;
        //FloorManager.FloorClearedFuntions -= Remove;
    }
    public void Remove(FloorManager floorManager)
    {
        TurnManager.RoundEndedFunctions -= AttemptToSpawnEnemy;
        FloorManager.FloorClearedFuntions -= Remove;
        Destroy(gameObject);
    }

}
