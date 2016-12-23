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

        public void SavePlayground() {
            SaveFile save = MakeSaveFile();
            string saveJSON = JsonUtility.ToJson(save);
            //Debug.Log(saveJSON);
            StartCoroutine(DataManager.Instance.SavePlayground(fileName.text, saveJSON));
            saveText.SetActive(true);
            saveText.GetComponent<Animation>().Play();
        }

        public SaveFile MakeSaveFile() {
            GameObject[] models = GameObject.FindGameObjectsWithTag("Models");
            List<ModelData> modelDatas = new List<ModelData>();

            string strToRemove = "(Clone)";

            foreach (GameObject model in models) {
                string newName = model.name.Replace(strToRemove,"");
                ModelData data = new ModelData(newName, model.transform.position, model.transform.rotation);
                modelDatas.Add(data);
            }

            SaveFile newSave = new SaveFile(modelDatas.ToArray());
            //Debug.Log(newSave.ToString());
            return newSave;
        }


    }
}
