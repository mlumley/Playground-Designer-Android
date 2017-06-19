using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the size of the playground
/// </summary>
public class GroundManager : MonoBehaviour {

    public GameObject ground;
    public GameObject tiles;
    public InputField width;
    public InputField length;

    /// <summary>
    /// Change how long the x axis of the ground is
    /// </summary>
    /// <param name="XSize">Length in meters</param>
    public void changeXSize(string XSize) {
        int x = int.Parse(XSize);
        ground.transform.localScale = new Vector3(x / 10f, ground.transform.localScale.y, ground.transform.localScale.z);
        tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(x, tiles.GetComponent<Renderer>().material.mainTextureScale.y);
    }

    /// <summary>
    /// Change how long the z axis of the ground is
    /// </summary>
    /// <param name="ZSize">Length in meters</param>
    public void changeZSize(string ZSize) {
        int z = int.Parse(ZSize);
        ground.transform.localScale = new Vector3(ground.transform.localScale.x, ground.transform.localScale.y, z / 10f);
        tiles.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tiles.GetComponent<Renderer>().material.mainTextureScale.x, z);
    }
}
