using UnityEngine;
using UnityEngine.UI;

public class Settings : UIButton
{
    //private PlayerControler playerControler;
    private GameObject pauseScreenBlocker;
    private bool isShown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        //playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();

        base.Awake();
        pauseScreenBlocker = RefrenceStorage.pauseScreenBlocker;
        GameManager.ResetGame += ResetState;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetState(GameManager gameManager)
    {
        isShown = false;
    }


    public override void Activate()
    {
        if (isShown)
        {
            pauseScreenBlocker.GetComponent<Image>().enabled = false;
            //pauseScreenBlocker.GetComponent<RectTransform>().sizeDelta = pauseScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;
            pauseScreenBlocker.transform.Find("Settings").gameObject.SetActive(false);
            isShown = false;
        }
        else
        {
            isShown = true;
            pauseScreenBlocker.GetComponent<Image>().enabled = true;
            pauseScreenBlocker.GetComponent<RectTransform>().sizeDelta = pauseScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;
            pauseScreenBlocker.transform.Find("Settings").gameObject.SetActive(true);
        }
    }
}