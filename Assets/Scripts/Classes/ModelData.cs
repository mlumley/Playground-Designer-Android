using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class ModelData{

    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public ModelData(string name, Vector3 position, Quaternion rotation, Vector3 scale) {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }

    public override string ToString() {
        return "Model name: " + name + ", Position: " + position.ToString() + " Rotation: " + rotation.ToString();
    }
}
