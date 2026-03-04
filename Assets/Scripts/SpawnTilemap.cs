using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnTilemap : MonoBehaviour
{
    private float tileWidth = 2, tileHeight, zLayer = 1000;
    private MapManager mapManager;
    public GameObject[] tiles;
    private float[] initialTileWeights = { 0.9f, 0.6f, 0.05f };
    private float[] tileProbabilities;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        tileWidth = mapManager.tileWidth;
        tileHeight = mapManager.tileHeight;
        BuildTileProbabilities(initialTileWeights);



        foreach (GameObject tile in tiles)
        {
            tile.transform.localScale = Vector3.one * tileWidth;
        }


        for (int i = -50; i <= 50; i++)
        {
            for (int j = -50; j <= 50; j++)
            {
                if (i == -50 || i == 50 || j == -50 || j == 50)
                {
                    spawnTile(tileWidth * j * 0.75f, i * tileHeight + Mathf.Abs(j) % 2 * tileHeight * 0.5f, tiles[2]);
                }
                else
                {
                    spawnRandomTile(tileWidth * j * 0.75f, i * tileHeight + Mathf.Abs(j) % 2 * tileHeight * 0.5f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void BuildTileProbabilities(float[] tileWeights)
    {
        float sumOfTileWeights = 0;
        for (int i = 0; i < tileWeights.Length; i++)
        {
            sumOfTileWeights += tileWeights[i];
        }
        tileProbabilities = new float[tileWeights.Length];
        tileProbabilities[0] = tileWeights[0] / sumOfTileWeights;
        for (int i = 1; i < tileWeights.Length; i++)
        {
            tileProbabilities[i] = tileWeights[i] / sumOfTileWeights + tileProbabilities[i - 1];
        }
    }

    private void spawnRandomTile(float x, float y)
    {
        float randomNumber = Random.Range(0, 1f);
        for (int i = 0; i < tiles.Length; i++)
        {
            if (randomNumber < tileProbabilities[i])
            {
                spawnTile(x, y, tiles[i]);
                break;
            }
        }
    }

    private void spawnTile(float x, float y, GameObject tile)
    {
        Instantiate(tile, new Vector3(x, y, zLayer), transform.rotation);
    }
}
