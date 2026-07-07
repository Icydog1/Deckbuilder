using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnchantress : Boss
{
    [SerializeField]
    GameObject orc, harpy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(18)
            ,() => Attack(16,6)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Block(30)
            ,() => Summon(orc)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Block(30)
            ,() => Summon(harpy)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(22)
            ,() => Attack(12,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => ApplyConditions(new Condition[]{ new Strength(3 + (3 * OverallStatistics.bossDifficulty)),new Speed(2 + (2 * OverallStatistics.bossDifficulty)) },"self or ally",Variables.gameInfinityValue,Variables.gameInfinityValue)
            //,() => ApplyCondition(,"self or ally",Variables.gameInfinityValue,Variables.gameInfinityValue)
        });
        movesSetOrder = new List<int>() { 0, 1, 3, 4, 0, 2, 3, 4 };

        maxHealth = 237 + (133 * OverallStatistics.bossDifficulty);
        XPValue = 30;

        base.Awake();
    }
}


