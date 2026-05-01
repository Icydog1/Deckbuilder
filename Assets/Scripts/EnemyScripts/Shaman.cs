using System.Collections.Generic;
public class Shaman : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(15)
            ,() => Attack(5,3,1,1,new Condition[] { new Strength(-3,2),new Finesse(-2, 2) })
        });
        moveSets.Add(new List<System.Action> {
            () => Move(15)
            ,() => Attack(5,3,1,1,new Condition[] { new Dexterity(-3, 2),new Speed(-2,2) })
        });

        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 50;
        base.Start();
    }
}