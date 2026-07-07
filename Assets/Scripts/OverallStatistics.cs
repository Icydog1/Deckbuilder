using System.Collections.Generic;
using UnityEngine;

public class OverallStatistics : MonoBehaviour
{
    public static int totalEnemiesKilled = 0;
    public static Dictionary<string, int> killedEnemies = new Dictionary<string, int>();
    public static int round = 0;
    public static int floor = 1;
    public static int bossDifficulty = 0;
    public static int enemyScaling = 0;
    public static int damageDealt = 0;
    public static int shuffles = 0;
    public static int roomsExplored = 0;

    public static void ResetStatistics()
    {
        totalEnemiesKilled = 0;
        killedEnemies.Clear();
        round = 0;
        floor = 1;
        bossDifficulty = 0;
        enemyScaling = 0;
        damageDealt = 0;
        shuffles = 0;
        roomsExplored = 0;
    }
}
