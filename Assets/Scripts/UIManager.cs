using UnityEngine;
using UnityEngine.UI;
using static Lootable;

public class UIManager : MonoBehaviour
{
    private Image rewardScreenBlocker, listDisplayerScreenBlocker, pauseScreenBlocker, deathScreenBlocker, compendiumScreenBlocker, mainMenuScreenBlocker, characterSelectScreenBlocker;
    //private Image listDisplayerScreenBlocker;
    //private Image pauseScreenBlocker;
    //private Image deathScreenBlocker;
    private RectTransform UITransform;
    private RectTransform rewardScreenBlockerTransform, listDisplayerScreenBlockerTransform, pauseScreenBlockerTransform, deathScreenBlockerTransform, compendiumScreenBlockerTransform, mainMenuScreenBlockerTransform, characterSelectScreenBlockerTransform;
    private bool isPaused, isDisplayingList, isGettingReward, isDead, isInCompendium, isOnMainMenu, isSelectingCharacter;
    public bool IsPaused { set { isPaused = value; UpdateScreen(); } }
    public bool IsDisplayingList { get { return isDisplayingList; } set { isDisplayingList = value; UpdateScreen(); } }
    public bool IsGettingReward { set { isGettingReward = value; UpdateScreen(); } }
    public bool IsDead { set { isDead = value; UpdateScreen(); } }
    public bool IsInCompendium { set { isInCompendium = value; UpdateScreen(); } }
    public bool IsOnMainMenu{ set { isOnMainMenu = value; UpdateScreen(); } }
    public bool IsSelectingCharacter { set { isSelectingCharacter = value; UpdateScreen(); } }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        deathScreenBlocker = RefrenceStorage.deathScreenBlocker.GetComponent<Image>();
        rewardScreenBlocker = RefrenceStorage.rewardScreenBlocker.GetComponent<Image>();
        listDisplayerScreenBlocker = RefrenceStorage.listDisplayerScreenBlocker.GetComponent<Image>();
        pauseScreenBlocker = RefrenceStorage.pauseScreenBlocker.GetComponent<Image>();
        compendiumScreenBlocker = RefrenceStorage.compendiumScreenBlocker.GetComponent<Image>();
        mainMenuScreenBlocker = RefrenceStorage.mainMenuScreenBlocker.GetComponent<Image>();
        characterSelectScreenBlocker = RefrenceStorage.characterSelectScreenBlocker.GetComponent<Image>();

        UITransform = RefrenceStorage.UI.GetComponent<RectTransform>();
        deathScreenBlockerTransform = deathScreenBlocker.GetComponent<RectTransform>();
        listDisplayerScreenBlockerTransform = listDisplayerScreenBlocker.GetComponent<RectTransform>();
        pauseScreenBlockerTransform = pauseScreenBlocker.GetComponent<RectTransform>();
        rewardScreenBlockerTransform = rewardScreenBlocker.GetComponent<RectTransform>();
        compendiumScreenBlockerTransform = compendiumScreenBlocker.GetComponent<RectTransform>();
        mainMenuScreenBlockerTransform = mainMenuScreenBlocker.GetComponent<RectTransform>();
        characterSelectScreenBlockerTransform = characterSelectScreenBlocker.GetComponent<RectTransform>();
        //listDisplayerScreenBlocker = GameObject.Find("ListDisplayerScreenBlocker").GetComponent<Image>();
        //pauseScreenBlocker = GameObject.Find("PauseScreenBlocker").GetComponent<Image>();
    }
    public void UpdateScreen()
    {
        rewardScreenBlocker.enabled = false;
        listDisplayerScreenBlocker.enabled = false;
        pauseScreenBlocker.enabled = false;
        deathScreenBlocker.enabled = false;
        compendiumScreenBlocker.enabled = false;
        characterSelectScreenBlocker.enabled = false;
        mainMenuScreenBlocker.enabled = false;
        if (isInCompendium)
        {
            compendiumScreenBlocker.enabled = true;
            compendiumScreenBlockerTransform.sizeDelta = UITransform.sizeDelta;
        }
        else if (isSelectingCharacter)
        {
            characterSelectScreenBlocker.enabled = true;
            characterSelectScreenBlockerTransform.sizeDelta = UITransform.sizeDelta;
        }
        else if (isOnMainMenu)
        {
            mainMenuScreenBlocker.enabled = true;
            mainMenuScreenBlockerTransform.sizeDelta = UITransform.sizeDelta;
        }
        else if (isPaused)
        {
            pauseScreenBlocker.enabled = true;
            pauseScreenBlockerTransform.sizeDelta = UITransform.sizeDelta;
        }
        else if (isDead)
        {
            deathScreenBlocker.enabled = true;
            deathScreenBlockerTransform.sizeDelta = UITransform.sizeDelta;
            //deathScreenBlocker.GetComponent<RectTransform>().sizeDelta = deathScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;
        }
        else if (isDisplayingList)
        {
            listDisplayerScreenBlocker.enabled = true;
            listDisplayerScreenBlockerTransform.sizeDelta = UITransform.sizeDelta;
        }
        else if (isGettingReward)
        {
            rewardScreenBlocker.enabled = true;
            rewardScreenBlockerTransform.sizeDelta = UITransform.sizeDelta;
        }
    }
}
