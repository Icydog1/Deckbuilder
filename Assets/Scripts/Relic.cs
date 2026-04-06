using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Relic : MonoBehaviour
{
    protected PlayerControler playerControler;
    protected RelicManager relicManager;
    protected VariableDisplayer countDisplay;

    protected string relicName;
    public string RelicName {  get { return relicName; } }


    protected bool isActive, isUnique;
    protected int count;
    protected int rarity = 1;
    public int Rarity { get { return rarity; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        relicManager = GameObject.Find("RelicManager").GetComponent<RelicManager>();
        countDisplay = transform.Find("RelicCountText").GetComponent<VariableDisplayer>();
        //GainRelic();
        //IncreaseCount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string GetRelicDescription()
    {
        List<string> descriptionList = new List<string>();
        playerControler.PlanDescription = descriptionList;
        playerControler.IsPlanning = true;
        OnGain();
        string displayedString = "";
        foreach (string text in descriptionList)
        {
            displayedString += text;
            displayedString += "\n";
        }
        playerControler.IsPlanning = false;
        return displayedString;
    }
    public void GainRelic()
    {
        playerControler.IsPlanning = false;
        OnGain();
    }
    public virtual void OnGain()
    {
        if (!playerControler.IsPlanning)
        {
            count = 1;
            if (!isUnique)
            {
                countDisplay.DisplayText(count);
            }
        }

    }
    public virtual void IncreaseCount()
    {
        count++;
        countDisplay.DisplayText(count);
    }

}


