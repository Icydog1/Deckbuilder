using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemy;
    private TurnManager turnManager;
    private float SpawnChance = 0.1f;
    private MapManager mapManager;
    private Vector2 OneToOnePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        OneToOnePos = mapManager.PosToOneToOne(transform.position);

        Instantiate(enemy, transform.position, transform.rotation);



        TurnManager.RoundEnded += AttemptToSpawnEnemy;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AttemptToSpawnEnemy(TurnManager turnManager)
    {
        if (Random.Range(0, 1f) <= SpawnChance)
        {
            if (mapManager.GetEntityOnHex(OneToOnePos) == null)
            {
                Instantiate(enemy, transform.position, transform.rotation);
            }
            else
            {
                Debug.Log("spawn obstructed");
            }
        }
        //Debug.Log("attempted to spawn");
    }
}
