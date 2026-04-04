using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : FigureStats
{
    //private TextMeshProUGUI specialPlayerText;

    //List<string> currentCondtions = new List<string>();

    private Camera camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        isPlayerUI = true;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
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
            planTextObject.transform.position = gameObject.transform.position + new Vector3(0, -0.25f * camera.orthographicSize / 5, 0);
        }
        else
        {
            planTextObject.transform.position = gameObject.transform.position + new Vector3(0, -0.55f * camera.orthographicSize / 5, 0);
        }
    }

}
