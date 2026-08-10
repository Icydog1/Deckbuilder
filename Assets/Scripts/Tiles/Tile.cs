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
    private Vector2 hexPos;
    public Vector2 HexPos { get { return hexPos; } set { hexPos = value; } }

    //public int distance;

    public int MoveCost { get { return moveCost; } set { moveCost = value; } }
    [SerializeField]
    private VariableDisplayer moveCostDisplay;
    public VariableDisplayer MoveCostDisplay { get { return moveCostDisplay; } }

    void Awake()
    {
        FloorManager.FloorClearedFuntions += Remove;
        if (moveCost < 1)
        {
            moveCost = 1;
        }
        transform.Find("TileUI").rotation = Quaternion.identity;
        hexPos = RefrenceStorage.mapManager.RectToHex(transform.position);
        RefrenceStorage.pathfinder.Tiles[hexPos] = this;
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
    public void Remove(FloorManager floorManager)
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        FloorManager.FloorClearedFuntions -= Remove;

    }
}
