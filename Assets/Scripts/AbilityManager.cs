using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private int abilityPower;
    public int AbilityPower {  get { return abilityPower; } set { abilityPower = value; } }

    private int selectedPower;
    public int SelectedPower { get { return selectedPower; } set { selectedPower = value; } }

    private List<Ability> abilities;
    [SerializeField]
    private GameObject abilityUIObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GainAbility(int cost, List<System.Action> actions)
    {
        //Ability newAbility = new Ability(cost, actions);
        Instantiate(abilityUIObject);
        Debug.Log(cost);
        Debug.Log(actions);
        Debug.Log(actions[0]);

        abilities.Add(new Ability(cost, actions));
        UpdateAbilitiesDescription();
        //UpdateAbilities();
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

    }
}
