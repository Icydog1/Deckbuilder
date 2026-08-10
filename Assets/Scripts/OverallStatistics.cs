using System.Collections.Generic;
using UnityEngine;

public class OverallStatistics : MonoBehaviour
{
    public static int totalEnemiesKilled = 0;
    public static Dictionary<string, int> killedEnemies = new Dictionary<string, int>();
    public static int round = 0;
    public static int floor = 1;
    public static bool bossFloor = false;
    public static int bossDifficulty = 0;
    public static int difficultyRound = 0;
    public static int enemyScaling = 0;
    public static float difficulty = 1;
    public static int damageDealt = 0;
    public static int shuffles = 0;
    public static int roomsExplored = 0;

    public static void ResetStatistics()
    {
        totalEnemiesKilled = 0;
        killedEnemies.Clear();
        round = 0;
        floor = 1;
        bossFloor = false;
        bossDifficulty = 0;
        difficultyRound = 0;
        enemyScaling = 0;
        difficulty = 1;
        damageDealt = 0;
        shuffles = 0;
        roomsExplored = 0;
    }
}
