using UnityEngine;
public class RestartGameButton : UIButton
{
    private GameManager gameManager;
    private VariableDisplayer typeText;

    //private string displayType = "On Move";
    //private bool changed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        base.Awake();
    }
    protected void Start()
    {
        //playerControler.MoveCostDisplaySetting = displayType;

    }


    // Update is called once per frame
    void Update()
    {

    }

    public override void Activate()
    {
        gameManager.ReStartGame();
    }
}
