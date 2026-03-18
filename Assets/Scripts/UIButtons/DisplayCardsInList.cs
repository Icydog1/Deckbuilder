using Unity.VisualScripting;
using UnityEngine;

public class DisplayCardsInList : UIButton
{
    private DeckManager deckManager;
    private GameObject listDisplayer;
    [SerializeField]
    private string listName;
    private bool isDisplaying;
    [SerializeField]
    private bool isRandomOrder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        listDisplayer = GameObject.Find("ListDisplayer");
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Activate()
    {
        deckManager.DisplayCardsInListByName(listName, listDisplayer.transform.position, 5, isRandomOrder);

    }
}