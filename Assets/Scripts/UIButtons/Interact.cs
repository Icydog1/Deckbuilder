using UnityEngine;

public class Interact : UIButton
{
    private PlayerControler playerControler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();

        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Activate()
    {

        //Debug.Log("Clicked button");
        playerControler.CurrentTile.GetComponent<Interactable>().Interacted();

    }
}
