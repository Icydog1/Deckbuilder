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
    [SerializeField]
    protected virtual int rarity => 1;
    public int Rarity { get { return rarity; } }
    protected string rarityColor;

    protected string relicDesription;
    protected List<ActionDescription> relicDescriptionList = new List<ActionDescription>();
    public List<ActionDescription> RelicDescriptionList { get { return relicDescriptionList; } set { relicDescriptionList = value; } }


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
        switch (rarity)
        {
            case 0:
                {
                    rarityColor = "<color=#B0B0B0>"; //grey
                    break;
                }
            case 1:
                {
                    rarityColor = "<color=#B0B0B0>"; //grey
                    break;
                }
            case 2:
                {
                    //rarityGlow.color = new Color(0, 0, 1, 0.5f);
                    rarityColor = "<color=#00D0FF>"; //blue
                    break;
                }
            case 3:
                {
                    //rarityGlow.color = new Color(1, 0.8f, 0, 1);
                    rarityColor = "<color=#FFCC00>"; //Gold
                    break;
                }
            default:
                {
                    //rarityGlow.color = new Color(0, 0, 0, 0f);
                    rarityColor = "<color=#B0B0B0>"; //grey
                    break;
                }
        }
    }

	// Update is called once per frame
	void Update()
    {
        
    }
    public IEnumerator GetRelicDescription(System.Action<string> callback)
    {
        if (relicDesription != null)
        {
            callback?.Invoke(rarityColor + relicName + "</color>" + "\n" + relicDesription);
        }
        else
        {
            //Debug.Log("GetRelicDescription");
            //actionManager.PlanToList = descriptionList;
            //playerControler.IsPlanning = true;
            relicDescriptionList = new List<ActionDescription>();
            yield return StartCoroutine(OnGain());
            //Debug.Log("descriptionList: " + descriptionList);
            string displayedString = "";
            foreach (ActionDescription text in relicDescriptionList)
            {
                //Debug.Log("relic description");
                displayedString += text.GetDescription();
                displayedString += "\n";
            }
            //playerControler.IsPlanning = false;
            //Debug.Log("displayedString: " + displayedString);
            displayedString = Regex.Replace(displayedString, "(. )([0-9]+)( .)", "${1}" + Var.relicIncreaseableNumberColor + "${2}</color>$3");

            callback?.Invoke(rarityColor + relicName + "</color>" + "\n" + displayedString);
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
                countDisplay.DisplayVariable(count);
            }
        }
        yield return null;

    }
    public virtual IEnumerator IncreaseCount()
    {
        count++;
        countDisplay.DisplayVariable(count);
        yield return null;
    }

}


