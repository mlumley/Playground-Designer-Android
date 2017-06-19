using UnityEngine;

/// <summary>
/// Controls whether the panels are hidden or not
/// </summary>
public class ModelPanelController : MonoBehaviour {

    private Animation ani;
    private bool isHid = false;
    public GameObject showHideButton;

    public bool IsHid {
        get {
            return isHid;
        }
    }

    // Use this for initialization
    void Start() {
        ani = GetComponent<Animation>();
    }

    /// <summary>
    /// Plays an animation to lower or raise the object panels
    /// </summary>
    public void ShowHidePanel() {
        if (isHid) {
            ani["HideShowPanel"].speed = -1;
            // Hard coded becuase it is much faster than finding the length of the clip
            ani["HideShowPanel"].time = 0.5f;
            ani.Play("HideShowPanel");
            isHid = false;
            showHideButton.GetComponent<SpriteSwapManager>().ChangeSprite(1);
        }
        else {
            ani["HideShowPanel"].speed = 1;
            ani["HideShowPanel"].time = 0;
            ani.Play("HideShowPanel");
            isHid = true;
            showHideButton.GetComponent<SpriteSwapManager>().ChangeSprite(2);
        }
    }

    /// <summary>
    /// Plays an animation to lower or raise the object panels
    /// </summary>
    /// <param name="position">1 to hide, 2 to raise</param>
    public void ShowHidePanel(int position) {
        if (position == 1) {
            ani["HideShowPanel"].speed = 1;
            ani["HideShowPanel"].time = 0;
            ani.Play("HideShowPanel");
            isHid = true;
            showHideButton.GetComponent<SpriteSwapManager>().ChangeSprite(2);
        }
        else {
            ani["HideShowPanel"].speed = -1;
            // Hard coded becuase it is much faster than finding the length of the clip
            ani["HideShowPanel"].time = 0.5f;
            ani.Play("HideShowPanel");
            isHid = false;
            showHideButton.GetComponent<SpriteSwapManager>().ChangeSprite(1);
        }
    }
}
