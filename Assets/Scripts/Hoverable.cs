using Unity.VisualScripting;
using UnityEngine;

public class Hoverable : MonoBehaviour
{
    private MouseManager mouseManager;
    private bool mouseOver;

    private float hoverDuration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (!GetComponent<DetectMouseOver>())
        {
            gameObject.AddComponent<DetectMouseOver>();
        }

    }
    void Start()
    {
        
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (hoverDuration)
        {

        }
    }

    public void StartedHoveringOver()
    {

    }

    public void StoppedHoveringOver()
    {

    }
}
