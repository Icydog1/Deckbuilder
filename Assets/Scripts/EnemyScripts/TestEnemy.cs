using UnityEngine;

public class TestEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start(); // runs the code from the base
                      // add your additional code here
    }

    // Update is called once per frame
    void Update()
    {
        MoveSet1();
    }

    public void MoveSet1()
    {
        MoveX(3);
    }
    public void MoveSet2()
    {

    }
}
