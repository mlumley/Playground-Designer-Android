using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroundManager : MonoBehaviour {

    public GameObject ground;
    public GameObject tiles;
    public InputField width;
    public InputField length;

    public void changeXSize(string XSize) {
        int x = int.Parse(XSize);
        //if (x > 0 && x <= 50) {
            ground.transform.localScale = new Vector3(x / 10f, ground.transform.localScale.y, ground.transform.localScale.z);
            tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(x, tiles.GetComponent<Renderer>().material.mainTextureScale.y);
        //}
        /*else if(x > 50) {
            ground.transform.localScale = new Vector3(50 / 10f, ground.transform.localScale.y, ground.transform.localScale.z);
            tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(50, tiles.GetComponent<Renderer>().material.mainTextureScale.y);
            width.text = "50";
        }
        else {
            ground.transform.localScale = new Vector3(1 / 10f, ground.transform.localScale.y, ground.transform.localScale.z);
            tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, tiles.GetComponent<Renderer>().material.mainTextureScale.y);
            width.text = "1";
        }*/
    }

    public void changeZSize(string ZSize) {
        int z = int.Parse(ZSize);
        //if (z > 0 && z <= 50) {
            ground.transform.localScale = new Vector3(ground.transform.localScale.x, ground.transform.localScale.y, z / 10f);
            tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tiles.GetComponent<Renderer>().material.mainTextureScale.x, z);
        //}
        /*else if (z > 50) {
            ground.transform.localScale = new Vector3(ground.transform.localScale.x, ground.transform.localScale.y, 50 / 10f);
            tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tiles.GetComponent<Renderer>().material.mainTextureScale.x, 50);
            length.text = "50";
        }
        else {
            ground.transform.localScale = new Vector3(ground.transform.localScale.x, ground.transform.localScale.y, 1 / 10f);
            tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tiles.GetComponent<Renderer>().material.mainTextureScale.x, 1);
            length.text = "1";
        }*/
    }
}
