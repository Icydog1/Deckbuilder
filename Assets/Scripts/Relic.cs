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

    public void GainRelic()
    {
        OnGain();
    }
    public virtual void OnGain()
    {

        count = 1;
        if (!isUnique)
        {

            countDisplay.DisplayText(count);
        }
    }
    public virtual void IncreaseCount()
    {
        count++;
        countDisplay.DisplayText(count);

    }

}


