using UnityEngine;
using UnityEngine.UI;

public class CompendiumTab : UIButton
{
    [SerializeField]
    private string text;
    protected override void Awake()
    {
        base.Awake();
    }
    public override void Activate()
    {
        RefrenceStorage.compendiumManager.OpenTab(text, gameObject);
    }
}