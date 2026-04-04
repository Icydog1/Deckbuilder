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
            changed = true;
        }
        if (displayType == "Always" && changed == false)
        {
            displayType = "Never";
            changed = true;
        }
        if (displayType == "Never" && changed == false)
        {
            displayType = "On Move";
            changed = true;
        }
        playerControler.MoveCostDisplaySetting = displayType;
        typeText.DisplayString(displayType);
        changed = false;
    }
}
