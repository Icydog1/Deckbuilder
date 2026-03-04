using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemy;
    private TurnManager turnManager;
    private float SpawnChance = 0.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();

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
            Instantiate(enemy, transform.position, transform.rotation);
        }
        //Debug.Log("attempted to spawn");
    }
}
