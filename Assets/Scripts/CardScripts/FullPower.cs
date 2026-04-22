using System.Collections;
using UnityEngine;

 public class FullPower : Card
{



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        topCost = 2;
        bottomCost = 2;
        base.Start(); // runs the code from the base
                      // add your additional code here
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); // runs the code from the base
                      // add your additional code here
    }


    public override void PrepareTop()
    {
        topActions.Add(() => playerControler.Attack(20));
        topActions.Add(() => playerControler.Attack(20));

    }

    public override void PrepareBottom()
    {
        bottomActions.Add(() => playerControler.Move(35));

    }

    /*
    public override IEnumerator PlayTop()
    {
        foreach (System.Action action in topActions)
        {
            action();
            yield return new WaitUntil(() => playerControler.nextAction == true);
            nextAction = false;
        }
    
    }
    
    public override void PlayTop()
    {
        //playerControler.AddToActionQueue(() => playerControler.Attack(1));
        //playerControler.AddToActionQueue(() => playerControler.Attack(3));
    }
    

    public override void PlayBottom()
    {
        playerControler.AddToActionQueue(() => playerControler.Move(4));
        playerControler.AddToActionQueue(() => playerControler.Move(2));
    }
    */
}
