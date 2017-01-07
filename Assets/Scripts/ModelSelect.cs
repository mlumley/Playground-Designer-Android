using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ModelSelect : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData) {
        gameObject.GetComponent<ObjectSelectionButton>().SelectThisObject();
    }

}
