using System.Collections.Generic;
using UnityEngine;

public class FigureStorage : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> Enemies {  get { return enemies; } set { enemies = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
