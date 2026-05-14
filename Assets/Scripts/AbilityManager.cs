using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class AbilityManager : MonoBehaviour
{
    private int abilityPower;
    public int AbilityPower {  get { return abilityPower; } set { abilityPower = value; avaliblePowerDisplay.DisplayText(abilityPower); } }

    private int selectedPower = 0;
    public int SelectedPower { get { return selectedPower; } }

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
    public IEnumerator SetSelectedPower(int power)
    {
        selectedPower = Mathf.Clamp(power, 0, abilityPower);
        //Debug.Log("Set power");
        yield return StartCoroutine(UpdateAbilitiesDescription());
        selectedPowerDisplay.DisplayText(selectedPower);
    }


    public IEnumerator ResetAbilityPower(PlayerControler playerControler)
    {
        AbilityPower = 0;
        //SelectedPower = 0;
        yield return StartCoroutine(SetSelectedPower(0));

    }

    public IEnumerator GainAbility(Ability ability)
    {
        //increasing ability cost doesnt work

        GameObject newAbilityUIObject = Instantiate(abilityUIObject, abilitiesDescriptions.transform);
        AbilityUI newAbilityUI = newAbilityUIObject.GetComponent<AbilityUI>();
        Ability newAbility = ability;
        newAbility.Gained();
        newAbility.AbilityUI = newAbilityUI;
        abilities.Add(newAbility);
        newAbilityUI.AbilityNumber = abilities.Count - 1;
        newAbilityUIObject.GetComponent<RectTransform>().anchoredPosition = abilitiesDescriptions.GetComponent<RectTransform>().anchoredPosition + new Vector2(-100, 450 - abilities.Count * 50);
        //Debug.Log("Gained ability");
        yield return StartCoroutine(UpdateAbilitiesDescription());
    }
    public void LoseAbility(Ability ability)
    {
        int abilityIndex = ability.AbilityUI.AbilityNumber;
        PlayerControler.PlayerTurnStartedFuntions -= ability.ResetAbilityCooldown;
        Destroy(ability.AbilityUI.gameObject);
        abilities.RemoveAt(abilityIndex);
        for(int i = abilityIndex; i < abilities.Count; i++)
        {
            AbilityUI abilityUI = abilities[i].AbilityUI;
            abilityUI.gameObject.GetComponent<RectTransform>().anchoredPosition = abilitiesDescriptions.GetComponent<RectTransform>().anchoredPosition + new Vector2(-100, 450 - (abilityUI.AbilityNumber) * 50);
            abilityUI.AbilityNumber--;

        }
    }
    public void ClearAllAbility(GameManager gameManager)
    {
        foreach (Ability ability in abilities)
        {
            PlayerControler.PlayerTurnStartedFuntions -= ability.ResetAbilityCooldown;
            Destroy(ability.AbilityUI.gameObject);
        }

        abilities.Clear();
    }


    public IEnumerator UpdateAbilitiesDescription()
    {
        //Debug.Log("start " + abilities.Count);
        //for (int i = 0; i < abilities.Count; i++)
        //{
        //    yield return StartCoroutine(abilities[i].UpdateDiscription(selectedPower));

        //}
        //Debug.Log("end " + abilities.Count);

        foreach (Ability ability in abilities)
        {
            yield return StartCoroutine(ability.UpdateDiscription(selectedPower));
            //convert x ability to y reasorce
        }
    }

    public void ActivateAbility(int abilityNumber)
    {
        StartCoroutine(abilities[abilityNumber].PreformAbility(selectedPower));
        //Debug.Log("activated " + abilityNumber);
    }
}
