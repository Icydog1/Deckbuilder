using UnityEngine;

public class EndCurrentAction : UIButton
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        playerControler.ManualEnd();
    }

}
