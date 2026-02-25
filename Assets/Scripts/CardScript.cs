using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardScript : MonoBehaviour
{
    public string location; //poisible locations, deck, hand, discard, exaust, shop, reward ect.
    //public Vector3 mousepos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnMouseDown()
    {
        Vector3 mousepos;
        mousepos = Input.mousePosition;
        Input.GetMouseButtonDown(0);
       
    }
    public void Drawn()
    {
        transform.position = Vector3.zero;
    }
    public void Played()
    {

    }
}
