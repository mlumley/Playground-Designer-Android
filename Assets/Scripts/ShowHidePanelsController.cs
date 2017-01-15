using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowHidePanelsController : MonoBehaviour {

    public Sprite up;
    public Sprite down;
    private bool isUp;
	
    public void ChangeSprite() {
        if (isUp) {
            gameObject.GetComponent<Image>().sprite = down;
            isUp = false;
        }
        else {
            gameObject.GetComponent<Image>().sprite = up;
            isUp = true;
        }
    }
}
