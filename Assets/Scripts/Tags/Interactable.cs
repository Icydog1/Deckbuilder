using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    //public static event Func<Interactable, IEnumerator> Interacted;
    public UnityEvent InteractedWith; // Shows up in Inspector


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    //public IEnumerator Interacted()
    //{

    //}
    public void Interacted()
    {
        //Debug.Log("interacted with button");

        InteractedWith.Invoke();
    }

}
