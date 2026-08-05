using UnityEngine;
public class StartGame : UIButton
{
    private MainMenuManager mainMenuManager;

    protected override void Awake()
    {
        mainMenuManager = RefrenceStorage.mainMenuManager;
        base.Awake();
    }
    public override void Activate()
    {
        RefrenceStorage.mouseManager.MouseOffObject(gameObject);
        mainMenuManager.StartGame();
    }
}
