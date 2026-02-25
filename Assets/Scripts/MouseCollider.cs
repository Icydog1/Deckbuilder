using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MouseCollider : MonoBehaviour
{
    public Collider2D mouseCollider;
    public GameManager gameManager;
    public Camera cam;
    private float zoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        zoom = cam.orthographicSize;
        transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Clickable>())
        {
            collision.gameObject.GetComponent<Clickable>().mouseOver();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

    }
    */
}
