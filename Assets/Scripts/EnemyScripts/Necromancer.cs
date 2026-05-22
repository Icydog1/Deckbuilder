using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Necromancer : Enemy
{
    [SerializeField]
    GameObject summon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(13)
            ,() => Attack(12, 5)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(17)
            ,() => Attack(19, 4)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Summon(summon)
        });
        movesSetOrder = new List<int>() { 0, 1, 2 };
        maxHealth = 113;
        XPValue = 6;
        base.Awake();
    }
}