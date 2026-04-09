
using Unity.VisualScripting;
using UnityEngine;

public class ChangeAbilityPower : UIButton
{
    private AbilityManager abilityManager;
    [SerializeField]
    private int powerIncrease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Activate()
    {
        abilityManager.SelectedPower += powerIncrease;
    }
}