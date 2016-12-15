using UnityEngine;
using System.Collections;
using System.IO;


public class MakePrefabs : MonoBehaviour {

    public void makePrefabs() {
        DirectoryInfo info = new DirectoryInfo("Assets/Resources/Models");
        FileInfo[] fileInfo = info.GetFiles();
        for (int i = 0; i < fileInfo.Length; i++) {
            if (!fileInfo[i].Name.Contains("meta")) {
                //Debug.Log("Models/" + file.Name.Replace(".skp", ""));
                GameObject model = (GameObject) Instantiate(Resources.Load("Models/" + fileInfo[i].Name.Replace(".skp", "")));
                model.AddComponent<BoxCollider>();
                model.GetComponent<BoxCollider>().center = Vector3.zero;
                model.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
                model.GetComponent<BoxCollider>().isTrigger = true;
                model.AddComponent<SelectedObjectCollision>();
#if UNITY_EDITOR
                //UnityEditor is not available in the built version! http://answers.unity3d.com/answers/316837/view.html
                UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/ModelPrefabs/" + fileInfo[i].Name.Replace(".skp", "") + ".prefab", model);
#endif 
                Destroy(model);
            }
        }


    }

}
