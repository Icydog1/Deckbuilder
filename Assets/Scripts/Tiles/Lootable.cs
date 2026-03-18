using UnityEngine;

public class Lootable : MonoBehaviour
{
    [SerializeField]
    private GameObject lootedTile;
    private bool isCard;
    private bool isRelic;
    private float raity;
    public float Raity { get { return raity; }}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Looted()
    {
        Instantiate(lootedTile,transform.position,transform.rotation);
        Destroy(gameObject);
    }
}
