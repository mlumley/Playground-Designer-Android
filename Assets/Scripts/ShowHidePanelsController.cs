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

    public void ChangeSprite(int sprite) {
        if (sprite == 1) {
            gameObject.GetComponent<Image>().sprite = down;
            isUp = false;
        }
        else if(sprite == 2) {
            gameObject.GetComponent<Image>().sprite = up;
            isUp = true;
        }
        else {
            Debug.Log("Error ChangeSprite not called with 1 or 2 or nothing");
        }
    }
}
