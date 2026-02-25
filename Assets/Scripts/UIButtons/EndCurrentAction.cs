using UnityEngine;

public class EndCurrentAction : UIButton
{
    private PlayerControler playerControler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        playerControler.manualEnd = true;
    }

}
