using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the visibility of the model panels
/// Specifically whether they are minimised or maximised
/// </summary>
public class SpriteSwapManager : MonoBehaviour {

    public Sprite up;
    public Sprite down;
    private bool isMaxmised;
	
    /// <summary>
    /// Toggle between minimised and maximised
    /// </summary>
    public void ChangeSprite() {
        if (isMaxmised) {
            gameObject.GetComponent<Image>().sprite = down;
            isMaxmised = false;
        }
        else {
            gameObject.GetComponent<Image>().sprite = up;
            isMaxmised = true;
        }
    }

    /// <summary>
    /// Selected minimised or maximised with 1 or 2 respectively
    /// </summary>
    /// <param name="sprite">1 is minimised and 2 is maximised</param>
    public void ChangeSprite(int sprite) {
        if (sprite == 1) {
            gameObject.GetComponent<Image>().sprite = down;
            isMaxmised = false;
        }
        else if(sprite == 2) {
            gameObject.GetComponent<Image>().sprite = up;
            isMaxmised = true;
        }
        else {
            Debug.Log("Error ChangeSprite not called with 1 or 2 or nothing");
        }
    }
}
