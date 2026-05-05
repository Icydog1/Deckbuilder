using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AbilityManager : MonoBehaviour
{
    private int abilityPower;
    public int AbilityPower {  get { return abilityPower; } set { abilityPower = value; avaliblePowerDisplay.DisplayText(abilityPower); } }

    private int selectedPower = 0;
    public int SelectedPower { get { return selectedPower; } set { selectedPower = Mathf.Max(Mathf.Min(value, abilityPower),0); UpdateAbilitiesDescription(); selectedPowerDisplay.DisplayText(selectedPower); } }

    private List<Ability> abilities = new List<Ability>();
    [SerializeField]
    private GameObject abilityUIObject;
    private GameObject abilitiesDescriptions;

    private VariableDisplayer avaliblePowerDisplay, selectedPowerDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abilitiesDescriptions = GameObject.Find("AbilitiesDescriptions");
        avaliblePowerDisplay = abilitiesDescriptions.transform.Find("AvaliblePowerDisplay").GetComponent<VariableDisplayer>();
        selectedPowerDisplay = abilitiesDescriptions.transform.Find("SelectedPowerDisplay").GetComponent<VariableDisplayer>();

        AbilityPower = 0;
        PlayerControler.PlayerTurnStarted += ResetAbilityPower;
        GameManager.ResetGame += ClearAllAbility;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetAbilityPower(PlayerControler playerControler)
    {
        AbilityPower = 0;
        SelectedPower = 0;
    }

    public void GainAbility(Ability ability)
    {
        //increasing ability cost doesnt work

        GameObject newAbilityUIObject = Instantiate(abilityUIObject, abilitiesDescriptions.transform);
        AbilityUI newAbilityUI = newAbilityUIObject.GetComponent<AbilityUI>();
        Ability newAbility = ability;
        newAbility.Gained();
        newAbility.AbilityUI = newAbilityUI;
        abilities.Add(newAbility);
        newAbilityUI.AbilityNumber = abilities.Count - 1;
        newAbilityUIObject.GetComponent<RectTransform>().anchoredPosition = abilitiesDescriptions.GetComponent<RectTransform>().anchoredPosition + new Vector2(-100, 450-abilities.Count * 50);

        UpdateAbilitiesDescription();
    }
    public void LoseAbility(Ability ability)
    {
        if (abilities.Contains(ability))
        {
            PlayerControler.PlayerTurnStarted -= ability.ResetAbilityCooldown;
            Debug.Log(ability.GetInstanceID());

            Debug.Log(abilities.IndexOf(abilities.Find(x => x.GetInstanceID() == ability.GetInstanceID())));
            abilities.RemoveAt(abilities.IndexOf(abilities.Find(x => x.GetInstanceID() == ability.GetInstanceID())));
            //var item = itemList.;


        }
    }
    public void ClearAllAbility(GameManager gameManager)
    {
        foreach (Ability ability in abilities)
        {
            PlayerControler.PlayerTurnStarted -= ability.ResetAbilityCooldown;

        }

        abilities.Clear();
    }


    public void UpdateAbilitiesDescription()
    {
        foreach (Ability ability in abilities)
        {
            ability.UpdateDiscription(selectedPower);
            //convert x ability to y reasorce
        }
    }

    public void ActivateAbility(int abilityNumber)
    {
        StartCoroutine(abilities[abilityNumber].PreformAbility(selectedPower));
        //Debug.Log("activated " + abilityNumber);
    }
}
