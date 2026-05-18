using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Image rewardScreenBlocker;
    private Image listDisplayerScreenBlocker;
    private Image pauseScreenBlocker;

    private bool isPaused, isDisplayingList, isGettingReward;
    public bool IsPaused { set { isPaused = value; UpdateScreen(); } }
    public bool IsDisplayingList { get { return isDisplayingList; } set { isDisplayingList = value; UpdateScreen(); } }
    public bool IsGettingReward { set { isGettingReward = value; UpdateScreen(); } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rewardScreenBlocker = GameObject.Find("RewardScreenBlocker").GetComponent<Image>();
        listDisplayerScreenBlocker = GameObject.Find("ListDisplayerScreenBlocker").GetComponent<Image>();
        pauseScreenBlocker = GameObject.Find("PauseScreenBlocker").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScreen()
    {
        rewardScreenBlocker.enabled = false;
        listDisplayerScreenBlocker.enabled = false;
        pauseScreenBlocker.enabled = false;
        if (isPaused)
        {
            pauseScreenBlocker.enabled = true;
            pauseScreenBlocker.GetComponent<RectTransform>().sizeDelta = pauseScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;
        }
        else if (isDisplayingList)
        {
            listDisplayerScreenBlocker.enabled = true;
            listDisplayerScreenBlocker.GetComponent<RectTransform>().sizeDelta = listDisplayerScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;
        }
        else if (isGettingReward)
        {
            rewardScreenBlocker.enabled = true;
            listDisplayerScreenBlocker.GetComponent<RectTransform>().sizeDelta = listDisplayerScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;

        }
    }
}
