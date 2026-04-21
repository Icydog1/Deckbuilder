
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeAbilityPower : UIButton
{
    private AbilityManager abilityManager;
    private MouseManager mouseManager;
    //private float timeHeld;
    [SerializeField]
    private int powerIncrease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();

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

    public IEnumerator HoldClick()
    {
        for (int timeHeldDown = 2; true; timeHeldDown++)
        {
            if (mouseManager.selectedObject == gameObject)
            {
                Activate();
            }
            yield return new WaitForSeconds(1 / Mathf.Pow((float)timeHeldDown * 10,0.5f));
        }
    }

}