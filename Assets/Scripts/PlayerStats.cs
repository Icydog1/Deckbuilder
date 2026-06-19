using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class PlayerStats : FigureStats
{
    //private TextMeshProUGUI specialPlayerText;

    //List<string> currentCondtions = new List<string>();
    protected GameObject levelAndXPTextObject;
    protected TextMeshProUGUI levelAndXPText;
    private Camera camera;
    string levelAndXPString, turnCountString;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        isPlayerUI = true;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        levelAndXPTextObject = transform.Find("LevelAndXPText").gameObject;

        levelAndXPText = levelAndXPTextObject.GetComponent<TextMeshProUGUI>();

        base.Awake();


        //Plan(testString);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void MovePlan()
    {
        if (noConditions)
        {
            planTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -35);

            //planTextObject.transform.position = gameObject.transform.position + new Vector3(0, -0.35f * camera.orthographicSize / 5, 0);
        }
        else
        {
            planTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);

            //planTextObject.transform.position = gameObject.transform.position + new Vector3(0, -0.7f * camera.orthographicSize / 5, 0);
        }
    }
    public override void SetLevelAndXP(int level, int potenialLevel, int XP, int XPThreshold)
    {
        levelAndXPString = "Level: " + level + "(" + potenialLevel + "), XP: " + XP + "/" + XPThreshold;
        SetLevelXPAndTurnCount();
    }
    public override void SetTurnCount(int turnCount)
    {
        turnCountString = ", Round: " + turnCount;
        SetLevelXPAndTurnCount();
        //levelAndXPText.SetText("Level: " + Level + "(" + potenialLevel + "), XP: " + XP + "/" + XPThreshold);
    }
    public void SetLevelXPAndTurnCount()
    {
        levelAndXPText.SetText(levelAndXPString + turnCountString);

    }

}
