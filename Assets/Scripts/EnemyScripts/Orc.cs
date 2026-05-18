using System;
using System.Collections;
using System.Collections.Generic;
public class Orc : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(25)
            ,() => Attack(5)
            ,() => Attack(5)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => ApplyCondition(new Strength(4))
        });

        initialMoves = new List<int>() { 0, 1, 0, 1, 0, 1};
        movesSetOrder = new List<int>() { 0 };
        maxHealth = 50;
        XPValue = 3;

        base.Awake();
    }
}