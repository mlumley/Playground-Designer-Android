using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class ModelData{

    public string name;
    public Vector3 position;
    public Quaternion rotation;

    public ModelData(string name, Vector3 position, Quaternion rotation) {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
    }

    public override string ToString() {
        return "Model name: " + name + ", Position: " + position.ToString() + " Rotation: " + rotation.ToString();
    }
}
