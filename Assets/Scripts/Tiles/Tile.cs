using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private int moveCost = 5;
    [SerializeField]
    private bool changeColor;
    [SerializeField]
    private bool isSpawner;
    private int zHeight = 1000;

    public int MoveCost { get { return moveCost; } }
    [SerializeField]
    private VariableDisplayer moveCostDisplay;
    public VariableDisplayer MoveCostDisplay { get { return moveCostDisplay; } }

    void Awake()
    {
        LevelManager.LevelCleared += Remove;
        if (moveCost < 1)
        {
            moveCost = 1;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, zHeight);

        if (changeColor)
        {
            float costColor = (111f / 255f) * (1 / (float)(Mathf.Sqrt(Mathf.Max(Mathf.Min(1/(2.25f-(float)(moveCost)/4), 1), (float)(moveCost - 4)))));
            //Debug.Log(costColor + "moveCost " + moveCost);
            float spawnerTint = 0;
            if (isSpawner)
            {
                spawnerTint = 0.25f;
            }
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(costColor + spawnerTint, costColor, costColor);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Remove(LevelManager levelManager)
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        LevelManager.LevelCleared -= Remove;

    }
}
