using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemy = new List<GameObject>();
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
        transform.position = new Vector3(transform.position.x, transform.position.y, spawnHeight);

        SpawnEnemy();

        if (enemy[0].name == "BaseEnemy")
        {
            Debug.Log("Warning: " + gameObject + " tried to summonBase enemy");
        }

        TurnManager.RoundEnded += AttemptToSpawnEnemy;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AttemptToSpawnEnemy(TurnManager turnManager)
    {
        if (Random.Range(0, 1f) <= spawnChance)
        {
            if (mapManager.GetEntityOnHex(OneToOnePos) == null)
            {
                SpawnEnemy();
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
        Instantiate(enemy[Random.Range(0,enemy.Count)], transform.position, Quaternion.identity);
    }

}
