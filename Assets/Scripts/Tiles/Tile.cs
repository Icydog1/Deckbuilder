using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private int moveCost = 5;

    private int zHeight = 1000;

    public int MoveCost { get { return moveCost; } }
    [SerializeField]
    private VariableDisplayer moveCostDisplay;
    public VariableDisplayer MoveCostDisplay { get { return moveCostDisplay; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, zHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
