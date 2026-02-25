using UnityEngine;
using UnityEngine.EventSystems;

public class TestScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("test");
    }
}
