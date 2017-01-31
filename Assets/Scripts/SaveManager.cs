using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using Assets.Scripts.Classes;


namespace Assets.Scripts {
    public class SaveManager : MonoBehaviour {

        public InputField fileName;
        public GameObject saveText;
        public InputField width;
        public InputField length;

        public void SavePlayground(bool withAnimation = true) {
            SaveFile save = MakeSaveFile();
            string saveJSON = JsonUtility.ToJson(save);
            //Debug.Log(saveJSON);
            StartCoroutine(DataManager.Instance.SavePlayground(fileName.text, saveJSON));
            if (withAnimation) {
                saveText.SetActive(true);
                saveText.GetComponent<Animation>().Play();
            }
        }

        public SaveFile MakeSaveFile() {

            GameObject[] models = GameObject.FindGameObjectsWithTag("Models");
            List<ModelData> modelDatas = new List<ModelData>();

            string strToRemove = "(Clone)";

            foreach (GameObject model in models) {
                string newName = model.name.Replace(strToRemove,"");
                ModelData data = new ModelData(newName, model.transform.position, model.transform.rotation, model.transform.localScale);
                modelDatas.Add(data);
            }

<<<<<<< Updated upstream
            SaveFile newSave = new SaveFile(modelDatas.ToArray());
=======
            GameObject[] photos = GameObject.FindGameObjectsWithTag("PhotoObject");
            List<PhotoData> photoDatas = new List<PhotoData>();

            //Debug.Log("Photos" + photos[0].ToString());

            foreach(GameObject photo in photos) {
                PhotoData data = new PhotoData(photo.transform.position, photo.transform.rotation, photo.transform.localScale, photo.GetComponent<SpriteRenderer>().sprite.texture.EncodeToPNG());
                photoDatas.Add(data);
            }

            int widthValue = 80;
            int lengthValue = 40;

            if (width.text != "")
                widthValue = int.Parse(width.text);
            if (length.text != "")
                lengthValue = int.Parse(length.text);
                

            SaveFile newSave = new SaveFile(widthValue, lengthValue, modelDatas.ToArray(), photoDatas.ToArray());
>>>>>>> Stashed changes
            //Debug.Log(newSave.ToString());
            return newSave;
        }


    }
}
