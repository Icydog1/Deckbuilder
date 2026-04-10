using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesDisplay : UIButton
{
    private GameObject abilitiesDiscriptions;
    private bool isShowing = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        abilitiesDiscriptions = GameObject.Find("AbilitiesDescriptions");

        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Activate()
    {
        if (isShowing)
        {
            //abilitiesDiscriptions.SetActive(false);
            isShowing = false;
        }
        else
        {
            //abilitiesDiscriptions.SetActive(true);
            isShowing = true;
        }
        //foreach (Transform child in abilitiesDiscriptions.transform)
        foreach (Transform child in abilitiesDiscriptions.transform.GetComponentsInChildren<Transform>())

        {
            if (child.gameObject.GetComponent<TextMeshProUGUI>())
            {
                child.gameObject.GetComponent<TextMeshProUGUI>().enabled = isShowing;
            }
            if (child.gameObject.GetComponent<Image>())
            {
                child.gameObject.GetComponent<Image>().enabled = isShowing;
            }
        }
    }
}
