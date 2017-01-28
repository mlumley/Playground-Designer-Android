using UnityEngine;
using System.Collections;

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

    public void ShowHidePanel() {
        if (isHid) {
            ani["HideShowPanel"].speed = -1;
            // Hard coded becuase it is much faster than finding the length of the clip
            ani["HideShowPanel"].time = 0.5f;
            ani.Play("HideShowPanel");
            isHid = false;
            showHideButton.GetComponent<ShowHidePanelsController>().ChangeSprite(1);
        }
        else {
            ani["HideShowPanel"].speed = 1;
            ani["HideShowPanel"].time = 0;
            ani.Play("HideShowPanel");
            isHid = true;
            showHideButton.GetComponent<ShowHidePanelsController>().ChangeSprite(2);
        }
    }
    public void ShowHidePanel(int position) {
        if (position == 1) {
            ani["HideShowPanel"].speed = 1;
            ani["HideShowPanel"].time = 0;
            ani.Play("HideShowPanel");
            isHid = true;
            showHideButton.GetComponent<ShowHidePanelsController>().ChangeSprite(2);
        }
        else {
            ani["HideShowPanel"].speed = -1;
            // Hard coded becuase it is much faster than finding the length of the clip
            ani["HideShowPanel"].time = 0.5f;
            ani.Play("HideShowPanel");
            isHid = false;
            showHideButton.GetComponent<ShowHidePanelsController>().ChangeSprite(1);
        }
    }
}
