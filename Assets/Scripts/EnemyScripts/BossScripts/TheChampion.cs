using System;
using System.Collections;
using System.Collections.Generic;

public class TheChampion : Boss
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(15)
            ,() => Attack(15)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(20)
            ,() => Attack(10,1,1,2)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Block(25)
            ,() => ApplyCondition(new Strength(8, -1))
            ,() => ApplyCondition(new Speed(6, -1))
        });
        movesSetOrder = new List<int>() { 0, 1, 2 };

        maxHealth = 300;
        XPValue = 30;

        base.Awake();
    }

}
