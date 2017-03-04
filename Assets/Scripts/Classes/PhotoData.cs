using UnityEngine;
using System.Collections;

[System.Serializable]
public class PhotoData {

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string name;
    private Texture2D image;

    public PhotoData(Vector3 position, Quaternion rotation, Vector3 scale, string name, Texture2D image) {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.name = name;
        this.image = image;
    }

    public Texture2D Image {
        get {
            return this.image;
        }
    }

    public override string ToString() {
        return "Photo: Position: " + position.ToString() + " Rotation: " + rotation.ToString() + " Scale: " + scale.ToString() + " Name: " + name + " Image: " + image.width;
    }

}
