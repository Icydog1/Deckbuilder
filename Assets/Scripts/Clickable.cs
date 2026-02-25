using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    
    private GameManager gameManager;
    private MouseManager mouseManager;
    private float height;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        height = transform.position.z;
    }

    public void OnMouseExit()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseManager.MouseOnObject(height, gameObject);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseManager.MouseOffObject(height, gameObject);

    }
    
}
