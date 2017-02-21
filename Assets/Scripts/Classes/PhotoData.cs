using UnityEngine;
using System.Collections;

[System.Serializable]
public class PhotoData {

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string imageURL;

    public PhotoData(Vector3 position, Quaternion rotation, Vector3 scale, string imageURL) {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.imageURL = imageURL;
    }

    public override string ToString() {
        return "Photo: Position: " + position.ToString() + " Rotation: " + rotation.ToString() + " Scale: " + scale.ToString();
    }

}
