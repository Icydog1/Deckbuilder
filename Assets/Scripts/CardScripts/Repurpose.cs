using UnityEngine;

public class Repurpose : Card
{
    public Repurpose() : base(2, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command()));
        currentActions.Add(new Action((currentTarget) => playerControler.Sacrifice(currentTarget)));
        currentActions.Add(new Action((currentTarget) => playerControler.GainTopEnergy(3)));
        currentActions.Add(new Action((currentTarget) => playerControler.GainBottomEnergy(3)));
    }

    public override void PrepareBottom()
    {

        currentActions.Add(new Action(() => playerControler.AddKeyword("Augment")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Speed(4))));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Speed(-2))));
        //discard card also
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