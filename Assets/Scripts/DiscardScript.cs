using System.Collections.Generic;
using UnityEngine;

public class DiscardScript : MonoBehaviour
{
    public List<GameObject> discardContents = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCard(GameObject card)
    {
        discardContents.Add(card);
        card.transform.parent = transform;
    }
    public void RemoveCard(GameObject card)
    {
        discardContents.Remove(card);
    }
}
