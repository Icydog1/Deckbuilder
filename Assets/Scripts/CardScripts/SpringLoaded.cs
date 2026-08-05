using System;
using UnityEngine;

public class SpringLoaded : Card
{
    public SpringLoaded() : base(2, 2, 1) { }


    [SerializeField]
    private GameObject boltShooter;

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Summon(boltShooter)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new NextTurnCards(-1, Var.infinityValue))));

    }

    public override void PrepareBottom()
    {
        
        currentActions.Add(new Action(() => playerControler.AddKeyword("Augment")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyConditions(new Condition[] { new Strength(2), new Speed(2) })));
        //currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Speed(2))));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Finesse(-2))));
        //currentActions.Add(new Action(() => ref currentTarget.ApplyCondition(new Accuracy(2)), null, ref playerControler.refEffectedFigures));

    }
}

//public class FunctionPasser : MonoBehaviour
//{
//    // 1. Stores a function that REQUIRES a MyExecutor script to run
//    public Action<Figure> storedFunction;

//    private void Awake()
//    {
//        // 2. Define WHAT the function does first (No script assigned yet)
//        storedFunction = (scriptInstance) =>
//        {
//            scriptInstance.DoSomething();
//        };
//    }

//    // 3. Call this later when you finally have the script instance
//    public void ExecuteNow(Figure scriptToRunIt)
//    {
//        storedFunction?.Invoke(scriptToRunIt);
//    }
//}