using System;
using UnityEngine;
public class SelectCharacter : UIButton
{
    private MainMenuManager mainMenuManager;
    [SerializeField]
    private Character selectedCharacter;
    public Character SelectedCharacter { get { return selectedCharacter; } }
    private Color backupBaseColor;
    public Color BackupBaseColor { get { return backupBaseColor; } set { backupBaseColor = value; } }

    protected override void Awake()
    {
        mainMenuManager = RefrenceStorage.mainMenuManager;
        base.Awake();
        backupBaseColor = baseColor;
    }
    public override void Activate()
    {
        mainMenuManager.SelectCharacter(this);
    }
}
