using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //showToolTip
    public void StartHoveringOver(GameObject gameObject)
    {
        transform.position = gameObject.transform.position;
        transform.GetChild(0).gameObject.SetActive(true);
        if (gameObject.GetComponent<Relic>())
        {
            SetText(gameObject.GetComponent<Relic>().GetRelicDescription());
        }
    }

    public void StopHoveringOver()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(text);
    }
}
