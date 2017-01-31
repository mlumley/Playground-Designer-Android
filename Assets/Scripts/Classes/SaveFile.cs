using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveFile {
    
    public ModelData[] models;
    public SaveFile(ModelData[] models) {
    public PhotoData[] photos;
    public int width;
    public int length;

    public SaveFile(int width, int length, ModelData[] models, PhotoData[] photos) {
        this.width = width;
        this.length = length;
        this.models = models;
    }

    public override string ToString() {
        return "Model Length: " + models.Length + ", " + modelInfo(models);
    }

    private string modelInfo(ModelData[] models) {
        string str = "";
        foreach(ModelData model in models) {
            str += model.ToString() + ", ";
        }
        return str;
    }
}
