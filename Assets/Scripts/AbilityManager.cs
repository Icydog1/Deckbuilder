using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private int abilityPower;
    public int AbilityPower {  get { return abilityPower; } set { abilityPower = value; } }

    private List<Ability> abilities;
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
        Ability newAbility = new Ability(cost, actions);
        abilities.Add(newAbility);
        UpdateAbilities();
    }

    public void UpdateAbilities()
    {
        foreach (Ability ability in abilities)
        {
            
        }
    }
    public void UpdateAbilitiesDescription()
    {
        foreach (Ability ability in abilities)
        {
            //convert x ability to y reasorce
        }
    }
}
