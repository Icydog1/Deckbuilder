using UnityEngine;

public class AbilitiesDisplay : UIButton
{
    private GameObject abilitiesDiscriptions;
    private bool isShowing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        abilitiesDiscriptions = GameObject.Find("AbilitiesDiscriptions");

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
            abilitiesDiscriptions.SetActive(false);

            isShowing = false;
        }
        else
        {
            abilitiesDiscriptions.SetActive(true);

            isShowing = true;

        }

    }
}
