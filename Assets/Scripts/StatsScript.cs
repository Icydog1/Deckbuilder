using UnityEngine;

public class StatsScript : MonoBehaviour
{
    private int health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetStatsOnEnemy(Enemy enemyScript)
    {
        health = enemyScript.health;
    }

    public void GetnerateEnemyStats()
    {

    }
}
