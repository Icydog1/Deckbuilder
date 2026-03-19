using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Image rewardScreenBlocker;
    private Image listDisplayerScreenBlocker;
    private Image pauseScreenBlocker;

    private bool isPaused, isDisplayingList, isGettingReward;
    public bool IsPaused { set { isPaused = value; UpdateScreen(); } }
    public bool IsDisplayingList { set { isDisplayingList = value; UpdateScreen(); } }
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
        }
        else if (isDisplayingList)
        {
            listDisplayerScreenBlocker.enabled = true;
        }
        else if (isGettingReward)
        {
            rewardScreenBlocker.enabled = true;
        }
    }
}
