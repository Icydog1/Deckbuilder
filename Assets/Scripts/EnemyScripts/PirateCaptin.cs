using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PirateCaptin : Enemy
{
    [SerializeField]
    GameObject summon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(18,true)
            ,() => Attack(7)
            ,() => Block(7)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Summon(summon,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(17,true)
            ,() => Attack(13)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(14,true)
            ,() => Attack(8)
            ,() => Attack(8)
        });
        movesSetOrder = new List<int>() { 0, 1, 2, 3 };
        maxHealth = 83;
        XPValue = 4;
        base.Awake();
    }
}