using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    protected List<Action> relicDescriptionList = new List<Action>();
    public List<Action> RelicDescriptionList { get { return relicDescriptionList; } set { relicDescriptionList = value; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        relicManager = GameObject.Find("RelicManager").GetComponent<RelicManager>();
        countDisplay = transform.Find("RelicCountText").GetComponent<VariableDisplayer>();
		actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
        relicName = this.name;
        relicName = relicName.Replace("(Clone)", "");
        relicName = Regex.Replace(relicName, "(.)([A-Z,0-9])", "$1 $2");
        //GainRelic();
        //IncreaseCount();
    }

	// Update is called once per frame
	void Update()
    {
        
    }
    public IEnumerator GetRelicDescription(System.Action<string> callback)
    {
        if (relicDesription != null)
        {
            callback?.Invoke(relicName + "\n" + relicDesription);

        }
        else
        {
            //Debug.Log("GetRelicDescription");
            //actionManager.PlanToList = descriptionList;
            //playerControler.IsPlanning = true;
            relicDescriptionList = new List<Action>();
            yield return StartCoroutine(OnGain());
            //Debug.Log("descriptionList: " + descriptionList);
            string displayedString = "";
            foreach (Action text in relicDescriptionList)
            {
                //Debug.Log("relic description");
                displayedString += text.GetDescription();
                displayedString += "\n";
            }
            //playerControler.IsPlanning = false;
            //Debug.Log("displayedString: " + displayedString);
            displayedString = Regex.Replace(displayedString, "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");

            callback?.Invoke(relicName + "\n" + displayedString);
        }
    }
    public IEnumerator GainRelic()
    {
        //playerControler.IsPlanning = false;
        relicDescriptionList = null;
        yield return StartCoroutine(OnGain());
    }
    public virtual IEnumerator OnGain()
    {
        if (relicDescriptionList == null)
        {
            count = 1;
            if (!isUnique)
            {
                countDisplay.DisplayText(count);
            }
        }
        yield return null;

    }
    public virtual IEnumerator IncreaseCount()
    {
        count++;
        countDisplay.DisplayText(count);
        yield return null;
    }

}


