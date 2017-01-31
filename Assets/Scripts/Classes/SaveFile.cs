using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveFile {
    
    public ModelData[] models;
    public PhotoData[] photos;

    public SaveFile(ModelData[] models, PhotoData[] photos) {
        this.models = models;
        this.photos = photos;
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
