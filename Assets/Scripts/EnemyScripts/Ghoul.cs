using System;
using System.Collections;
using System.Collections.Generic;
public class Ghoul : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(8)
            ,() => Attack(12,1,1,1,new Condition[] { new Dexterity(-4,2)})
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(16)
            ,() => Attack(8,1,1,1,new Condition[] { new Speed(-4,2) })
        });

        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 43;
        XPValue = 8;
        base.Awake();
    }
}