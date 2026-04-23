using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemy = new List<GameObject>();
    private int activationDelay = 5;
    private int tunsTillActive;
    private TurnManager turnManager;
    private float spawnChance = 0.1f;
    private float spawnHeight = 12;
    private MapManager mapManager;
    private Vector2 OneToOnePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

        TurnManager.RoundEnded += AttemptToSpawnEnemy;
        //LevelManager.LevelCleared += Remove;
        tunsTillActive = activationDelay;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AttemptToSpawnEnemy(TurnManager turnManager)
    {
        tunsTillActive--;
        if (tunsTillActive <= 0 && Random.Range(0, 1f) <= spawnChance)
        {
            if (mapManager.GetEntityOnHex(OneToOnePos) == null)
            {
                SpawnEnemy();
                tunsTillActive = activationDelay;
            }
            else
            {
                Debug.Log("spawn obstructed");
            }
        }
        //Debug.Log("attempted to spawn");
    }

    public void SpawnEnemy()
    {
        Instantiate(enemy[Random.Range(0,enemy.Count)], new Vector3(transform.position.x, transform.position.y, spawnHeight), Quaternion.identity);
    }
    public void OnDestroy()
    {
        TurnManager.RoundEnded -= AttemptToSpawnEnemy;
    }
    public void Remove(LevelManager levelManager)
    {
        TurnManager.RoundEnded -= AttemptToSpawnEnemy;
        LevelManager.LevelCleared -= Remove;
        Destroy(gameObject);
    }

}
