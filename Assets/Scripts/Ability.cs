using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    protected int cost;
    protected List<System.Action> abilities = new List<System.Action>();
    protected PlayerControler playerControler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Ability(int abilitiesCost, List<System.Action> preformedAbilities)
    {
        abilities = preformedAbilities;
        cost = abilitiesCost;

    }
}
