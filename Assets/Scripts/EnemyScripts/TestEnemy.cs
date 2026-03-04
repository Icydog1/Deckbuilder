using UnityEngine;

public class TestEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        movesets = new string[] {"MoveSet1", "MoveSet2", "MoveSet3"};
        maxHealth = 10;
        base.Start(); // runs the code from the base
                      // add your additional code here
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); // runs the code from the base
    }

    public void MoveSet1()
    {
        Move(1);
        //EndTurn();
    }
    public void MoveSet2()
    {
        Move(2);
        //EndTurn();

    }

    public void MoveSet3()
    {
        Move(3);
        //EndTurn();

    }
}
