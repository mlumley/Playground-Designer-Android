using UnityEngine;
using System.Collections;

public class GroundManager : MonoBehaviour {

    public GameObject ground;
    public GameObject tiles;

    public void changeXSize(string XSize) {
        int x = int.Parse(XSize);
        ground.transform.localScale = new Vector3(x, ground.transform.localScale.y, ground.transform.localScale.z);
        tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(x, tiles.GetComponent<Renderer>().material.mainTextureScale.y);
    }

    public void changeZSize(string ZSize) {
        int z = int.Parse(ZSize);
        ground.transform.localScale = new Vector3(ground.transform.localScale.x, ground.transform.localScale.y, z);
        tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tiles.GetComponent<Renderer>().material.mainTextureScale.x, z);
    }
}
