using System.Collections.Generic;
using UnityEngine;

public class SpawnTilemap : MonoBehaviour
{
    private float tileWidth = 2, tileHeight;
    private MapManager mapManager;
    public List<GameObject> tiles = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        tileWidth = mapManager.tileWidth;
        tileHeight = mapManager.tileHeight;

        foreach (GameObject tile in tiles)
        {
            tile.transform.localScale = Vector3.one * tileWidth;
        }


        for (int i = -50; i < 50; i++)
        {
            for (int j = -50; j < 50; j++)
            {
                spawnTile(tileWidth * j * 0.75f, i * tileHeight + Mathf.Abs(j) % 2 * tileHeight * 0.5f, tiles[0]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void spawnTile(float x, float y, GameObject tile)
    {
        Instantiate(tile, new Vector3(x, y, tile.transform.position.z), transform.rotation);
    }
}
