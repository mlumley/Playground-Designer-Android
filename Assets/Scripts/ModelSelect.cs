using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ModelSelect : MonoBehaviour, IPointerDownHandler, IDragHandler {
    public void OnDrag(PointerEventData eventData) { }

    public void OnPointerDown(PointerEventData eventData) {
        gameObject.GetComponent<ObjectSelectionButton>().SelectThisObject();
    }

}
