using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Relic : MonoBehaviour
{
    protected PlayerControler playerControler;
    protected RelicManager relicManager;
    protected VariableDisplayer countDisplay;
	protected ActionManager actionManager;

	protected string relicName;
    public string RelicName {  get { return relicName; } }
    protected bool isActive, isUnique;
    public bool IsUnique { get { return isUnique; } }

    protected int count;
    protected int rarity = 1;
    public int Rarity { get { return rarity; } }

    protected string relicDesription;
    protected List<string> relicDescriptionList = new List<string>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        relicManager = GameObject.Find("RelicManager").GetComponent<RelicManager>();
        countDisplay = transform.Find("RelicCountText").GetComponent<VariableDisplayer>();
		actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();

		//GainRelic();
		//IncreaseCount();
	}

	// Update is called once per frame
	void Update()
    {
        
    }
    public string GetRelicDescription()
    {
        if (relicDesription != null)
        {
            return relicName + "\n" + relicDesription;
        }
        //Debug.Log("GetRelicDescription");
        //actionManager.PlanToList = descriptionList;
        //playerControler.IsPlanning = true;
        relicDescriptionList = new List<string>();
        OnGain();
        //Debug.Log("descriptionList: " + descriptionList);

        string displayedString = "";
        foreach (string text in relicDescriptionList)
        {
            displayedString += text;
            displayedString += "\n";
        }
        //playerControler.IsPlanning = false;
        //Debug.Log("displayedString: " + displayedString);

        return relicName + "\n" + displayedString;
    }
    public void GainRelic()
    {
        //playerControler.IsPlanning = false;
        relicDescriptionList = null;
        OnGain();
    }
    public virtual void OnGain()
    {
        if (relicDescriptionList == null)
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


