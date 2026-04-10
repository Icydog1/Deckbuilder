using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : UIButton
{
    private TextMeshProUGUI abilityText;
    private AbilityManager abilityManager;
    private Image blocker;

    private int abilityNumber;
    public int AbilityNumber {  set { abilityNumber = value; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        abilityText = transform.Find("AbilityText").GetComponent<TextMeshProUGUI>();
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();
        blocker = transform.Find("Blocker").GetComponent<Image>();
        base.Awake();
    }
    void Start()
    {


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
        string displayedString = "";
        foreach (string text in description)
        {
            displayedString += text;
            displayedString += "\n";
        }
        abilityText.text = displayedString;
    }
    public void DisplayUsed(bool isUsed)
    {
        blocker.enabled = isUsed;
        GetComponent<DetectMouseOver>().enabled = !isUsed;
    }
}
