using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class RelicManager : MonoBehaviour
{
    private PlayerControler playerControler;
    private CameraScript cameraScript;
    private GameObject relicDisplayer;


    private Dictionary<string, Relic> relics = new Dictionary<string, Relic>();
    public Dictionary<string, Relic> Relics { get { return relics; } set { relics = value; } }

    private List<GameObject> relicObjects = new List<GameObject>();
    public List<GameObject> RelicObjects { get { return relicObjects; } set { relicObjects = value; } }

    private float relicSize = 1f, relativeSpaceBetweenRelics = 0.13f;
    private int rowLimit = 25;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        relicDisplayer = GameObject.Find("RelicDisplayer");
        GameManager.GameStarted += TestGainRelic;
        rowLimit = Mathf.FloorToInt((float)rowLimit / relicSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TestGainRelic(GameManager gameManager)
    {
        GameObject tester = GameObject.Find("Tester");
        List<GameObject> testObjects = new List<GameObject>();
        for (int i = 0; i < tester.transform.childCount; i++)
        {
            testObjects.Add(tester.transform.GetChild(i).gameObject);
        }
        foreach (GameObject obj in testObjects)
        {
            GainRelic(obj);
        }

    }
    public void GainRelic(GameObject relic)
    {
        Relic relicSript = relic.GetComponent<Relic>();

        if (relics.ContainsKey(relicSript.RelicName))
        {
            Relic newRelicSript = relics[relicSript.RelicName];
            newRelicSript.IncreaseCount();
            Destroy(relic);
        }
        else 
        {
            relicSript.OnGain();
            relicObjects.Add(relic);
            relics.Add(relicSript.RelicName, relicSript);
            relic.transform.SetParent(relicDisplayer.transform);
            OrderRelics();
            //Debug.Log(relics[relicSript.name].name);
        }
    }
    public void OrderRelics()
    {
        float horizontalSpaceBetweenRelics = relativeSpaceBetweenRelics * relicSize * cameraScript.widthHeightRatio;
        float VerticalSpaceBetweenRelics = relativeSpaceBetweenRelics * relicSize * cameraScript.widthHeightRatio;
        int numberOfRelics = relicObjects.Count;
        int rowsCount = Mathf.CeilToInt(relicObjects.Count / rowLimit);
        foreach (GameObject relic in relicObjects)
        {
            relic.transform.localScale = new Vector3(relicSize, relicSize, 1);
            int row = Mathf.FloorToInt(relicObjects.IndexOf(relic) / rowLimit);
            int column = relicObjects.IndexOf(relic) % rowLimit;
            relic.transform.position = new Vector3(relicDisplayer.transform.position.x + (column - (rowLimit / 2)) * horizontalSpaceBetweenRelics, relicDisplayer.transform.position.y - row * VerticalSpaceBetweenRelics, relic.transform.position.z);
            //Debug.Log(relic.transform.position);
        }
    }
}
