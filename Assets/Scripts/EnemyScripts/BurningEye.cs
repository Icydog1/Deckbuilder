using System;
using System.Collections;
using System.Collections.Generic;
public class BurningEye : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(12,8,1,1,new Condition[] { new Speed(-10,1) })
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(21,1,Var.infinityValue)
            ,() => Attack(7,6)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(7,13)
            ,() => Heal(10)

        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(9)
            ,() => Attack(9,8)
        });


        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 32;
        XPValue = 6;
        base.Awake();
    }
}