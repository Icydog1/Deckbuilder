using System.Collections;
using UnityEngine;

 public class TestCardScript : Card
{



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start(); // runs the code from the base
                      // add your additional code here
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); // runs the code from the base
                      // add your additional code here
    }

    public override IEnumerator PlayTop()
    {
        playerControler.AttackX(1, 100);
        yield return new WaitUntil(() => currentStep == 1);
        playerControler.AttackX(3, 100);
        yield return new WaitUntil(() => currentStep == 2);
        //Debug.Log("top played fully");

        DonePlaying();

    }


    public override IEnumerator PlayBottom()
    {
        playerControler.MoveX(4);
        yield return new WaitUntil(() => currentStep == 1);
        playerControler.MoveX(3);
        yield return new WaitUntil(() => currentStep == 2);
        //Debug.Log("bottom played fully");
        DonePlaying();
    }
}
