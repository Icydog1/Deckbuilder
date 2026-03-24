using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyUi : FigureStats
{
    public GameObject planTextObject;
    private TextMeshProUGUI planText;
    public override void Awake()
    {
        base.Awake();

        planText = planTextObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Plan(List<string> moves)
    {
        if (noConditions)
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0.25f , 0);
        }
        else
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(-0.5f, 0.0f, 0);
        }
        string movesDisplay = "";
        foreach (string move in moves)
        {
            if (movesDisplay == "")
            {
                movesDisplay = move;
            }
            else
            {
                movesDisplay += ", " + move;
            }
        }
        planText.SetText("Plan: " + movesDisplay);
    }
}
