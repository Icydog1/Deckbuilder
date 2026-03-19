using UnityEngine;

public class UIButton : MonoBehaviour
{
    protected PlayerControler playerControler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Activate()
    {
        Debug.Log("Base UI Activated");
    }

}
