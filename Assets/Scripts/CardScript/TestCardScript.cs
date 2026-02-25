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
        Debug.Log("top played fully");
        yield return new WaitUntil(() => currentStep == 1);
        playerControler.MoveX(1);
        yield return new WaitUntil(() => currentStep == 2);
        DonePlaying();

    }


    public override IEnumerator PlayBottom()
    {
        Debug.Log("bottom played fully");
        yield return new WaitUntil(() => currentStep == 1);
        playerControler.MoveX(1);
        yield return new WaitUntil(() => currentStep == 2);
        DonePlaying();
    }
}
