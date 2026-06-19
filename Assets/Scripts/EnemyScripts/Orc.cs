using System;
using System.Collections;
using System.Collections.Generic;
public class Orc : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(29)
            ,() => Attack(4,1,1,2)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => ApplyCondition(new Strength(4))
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Block(19)
        });

        initialMoves = new List<int>() { 0, 1, 0, 1, 0, 1};
        movesSetOrder = new List<int>() { 0, 2};
        maxHealth = 58;
        XPValue = 3;

        base.Awake();
    }
}