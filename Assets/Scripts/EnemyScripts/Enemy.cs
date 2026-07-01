using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.Rendering.GPUSort;

public class Enemy : AIFigure
{
    public override void Awake()
    {
        team = 1;
        isEnemy = true;
        base.Awake();
    }


    public override IEnumerator Die()
    {
        yield return playerControler.StartCoroutine(playerControler.KilledEnemy(XPValue));
        if (OverallStatistics.killedEnemies.ContainsKey(figureName))
        {
            OverallStatistics.killedEnemies[figureName]++;
        }
        else
        {
            OverallStatistics.killedEnemies.Add(figureName, 1);
        }
        OverallStatistics.totalEnemiesKilled++;
        yield return gameManager.StartCoroutine(base.Die());
    }
}
