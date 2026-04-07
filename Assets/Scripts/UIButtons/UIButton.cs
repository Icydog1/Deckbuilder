using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    private Color clickedColor = new Color(0.8f, 0.8f, 0.8f);
    private Color baseColor;
    public Color ClickedColor { get { return clickedColor; }}
    public Color BaseColor { get { return baseColor; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        baseColor = transform.Find("Image").gameObject.GetComponent<Image>().color;


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
