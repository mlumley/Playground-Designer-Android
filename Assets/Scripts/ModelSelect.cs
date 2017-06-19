using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Override PointerDown and OnDrag for models
/// </summary>
public class ModelSelect : MonoBehaviour, IPointerDownHandler, IDragHandler {
    public void OnDrag(PointerEventData eventData) { }

    public void OnPointerDown(PointerEventData eventData) {
        gameObject.GetComponent<ObjectSelectionButton>().SelectThisObject();
    }

}
