using System.Collections.Generic;
using UnityEngine;

public class OverallStatistics : MonoBehaviour
{
    private int totalEnemiesKilled;
    public int TotalEnemiesKilled { get { return totalEnemiesKilled; } set { totalEnemiesKilled = value; } }

    private Dictionary<string, int> killedEnemies = new Dictionary<string, int>();
    public Dictionary<string, int> KilledEnemies { get { return killedEnemies; } set { killedEnemies = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
