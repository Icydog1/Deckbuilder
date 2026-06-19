using UnityEngine;
using UnityEngine.UI;

public class CloseCompendium : UIButton
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override void Activate()
    {
        RefrenceStorage.mouseManager.MouseOffObject(gameObject);
        RefrenceStorage.compendiumManager.HideCompendium();
    }
}