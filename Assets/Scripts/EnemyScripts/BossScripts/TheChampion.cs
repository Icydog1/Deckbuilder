using System;
using System.Collections;
using System.Collections.Generic;

public class TheChampion : Boss
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(18)
            ,() => Attack(16)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(22)
            ,() => Attack(12,1,1,2)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Block(23)
            ,() => ApplyCondition(new Strength(8, -1))
            ,() => ApplyCondition(new Speed(6, -1))
        });
        movesSetOrder = new List<int>() { 0, 1, 2 };

        maxHealth = 337;
        XPValue = 32;

        base.Awake();
    }

}
