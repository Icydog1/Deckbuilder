using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityUI : UIButton
{
    private TextMeshProUGUI abilityText;
    private AbilityManager abilityManager;
    private int abilityNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        //playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        //typeText = transform.Find("ShowMoveCostText").GetComponent<VariableDisplayer>();
        base.Awake();
    }
    void Start()
    {
        abilityText = transform.Find("AbilityText").GetComponent<TextMeshProUGUI>();
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        abilityManager.ActivateAbility(abilityNumber);
    }

    public void DisplayText(List<string> description)
    {
        string text = description.ToString();

        abilityText.text = text;
    }

}
