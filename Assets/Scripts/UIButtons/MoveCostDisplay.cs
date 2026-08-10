    using UnityEngine;
using static UnityEngine.Rendering.BoolParameter;

public class MoveCostDisplay : UIButton
{
    private PlayerControler playerControler;
    private VariableDisplayer typeText;

    private string displayType = "On Move";
    private bool changed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        typeText = transform.Find("ShowMoveCostText").GetComponent<VariableDisplayer>();
        base.Awake();
    }
    protected void Start()
    {
        playerControler.MoveCostDisplaySetting = displayType;

    }


    // Update is called once per frame
    void Update()
    {

    }

    public override void Activate()
    {
        if (displayType == "On Move" && changed == false)
        {
            displayType = "Always";
            //changed = true;
        }
        else if (displayType == "Always" && changed == false)
        {
            displayType = "Move Field";

            //changed = true;
        }
        else if (displayType == "Move Field" && changed == false)
        {
            displayType = "Never";
            //changed = true;
        }

        else if (displayType == "Never")
        {
            displayType = "On Move";

            //DisplayMoveField
        }
        playerControler.MoveCostDisplaySetting = displayType;
        typeText.DisplayString(displayType);
        changed = false;
    }
}
