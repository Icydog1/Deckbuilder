using System.Collections;
using UnityEngine;

 public class TestCardScript : Card
{



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        topActions.Add(() => playerControler.Attack(4));
        topActions.Add(() => playerControler.Attack(4));

        bottomActions.Add(() => playerControler.Move(4));
        bottomActions.Add(() => playerControler.Move(2));

        topDescription = "Attack 4";


        base.Start(); // runs the code from the base
                      // add your additional code here
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); // runs the code from the base
                      // add your additional code here
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
