using UnityEngine;
public class GoToCharacterSelect : UIButton
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
        mainMenuManager.CharacterSelect();
    }
}
