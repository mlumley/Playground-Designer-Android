using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour {

	protected static PlayerManager _instance;

	public static PlayerManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of PlayerManager in the scene.");
				}
			}
			return _instance;
		}
	}

	[HideInInspector]
	public Transform currentObject;

	public Camera PlayerCamera;

	public bool isObjSelected = false;

	private float doubleClickTime = .4f;
	private float lastClickTime = -10f;

	private bool HitRotateCirlce = false;
	private Vector3 RotateCircleHitPreviousPoint;

	[HideInInspector]
	public bool MoveableUISelected = false;

	bool hasHitUI = false;

	void Update ()
	{
		if(Input.GetMouseButtonDown(0))
		{
			PointerEventData pointer1 = new PointerEventData (EventSystem.current);
			pointer1.position = Input.mousePosition;
			List<RaycastResult> raycastResults1 = new List<RaycastResult> ();
			EventSystem.current.RaycastAll (pointer1, raycastResults1);

            Debug.Log("Mouse Down");
			if (raycastResults1.Count > 0)
			{
				foreach (RaycastResult result in raycastResults1)
				{
                    Debug.Log("Hit " + result.gameObject.name);
					if(result.gameObject == null)
					{
						break;
					}

					if (result.gameObject.layer == LayerMask.NameToLayer ("UI"))
					{
						hasHitUI = true;
						break;
					}
					else if (result.gameObject.layer == LayerMask.NameToLayer ("MoveableUI"))
					{
						if(currentObject == null)
							continue;

						if (result.gameObject.transform.position != currentObject.position)
						{
							SetSelectableToNull ();
							hasHitUI = false;
							break;
						}

						hasHitUI = false;
						break;
					}
				}
			}
			else
			{
                Debug.Log("Hit nothing");
				hasHitUI = false;

				if (currentObject)
				{
					if (MoveableUISelected)
					{
						SetSelectableToNull();

						return;
					}

					RaycastHit hitInfo4;
					if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo4, Mathf.Infinity, 1 << LayerMask.NameToLayer ("GridCollider")))
					{
						if (!MoveableUISelected)
						{
							RaycastHit hitInfo6;
							if (RotateCirclesRenderer.Instance.HitRotateCircleCheck (hitInfo4.point))
							{
								HitRotateCirlce = true;
								RotateCircleHitPreviousPoint = hitInfo4.point;
							}
							else if (!Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo6, Mathf.Infinity, 1 << LayerMask.NameToLayer ("DesignObject")))
							{
								SetSelectableToNull();
							}
						}
						else
						{
							HitRotateCirlce = false;
						}
					}
				}
			}

			float timeDelta = Time.time - lastClickTime;

			//if(timeDelta < doubleClickTime)
			//{
				PointerEventData pointer = new PointerEventData(EventSystem.current);
				pointer.position = Input.mousePosition;
				List<RaycastResult> raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll (pointer, raycastResults);

                //Debug.Log("Double click");
				//if (raycastResults.Count > 0)
				//{
				//	if (raycastResults [0].gameObject.layer == LayerMask.NameToLayer ("MoveableUI"))
				//	{
				//		IUISelectable selectable = raycastResults [0].gameObject.GetComponent<IUISelectable> ();

				//		if (selectable == null)
				//		{
				//			selectable = raycastResults [0].gameObject.transform.parent.GetComponent<IUISelectable> ();
				//			currentObject = raycastResults [0].gameObject.transform.parent;
				//		}
				//		else
				//		{
				//			currentObject = raycastResults [0].gameObject.transform;
				//		}
							
				//		selectable.Select(true);
				//		MoveableUISelected = true;
				//	}
				//}
				//else
				{
                    Debug.Log("Single click");
					RaycastHit hitInfo;

					if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer ("DesignObject")))
					{
						currentObject = hitInfo.transform;
                        SelectedObjectCircleRenderer.Instance.SetSelectedObject (currentObject);
					}
				}

				lastClickTime = 0f;

			}
			else
			{
				lastClickTime = Time.time;
			}
		//}

		if (lastClickTime > 0f && Time.time - lastClickTime > doubleClickTime)
		{
			if (EventSystem.current.IsPointerOverGameObject ()) return;

			lastClickTime = 0f;
		}
			
		if (Input.GetMouseButton(0) && !hasHitUI)
		{
            //Debug.Log("Hit object");
			if (MoveableUISelected && currentObject)
			{
                Debug.Log("move");
                //currentObject.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                //    currentObject.position = hit.point;
                //}

                isObjSelected = true;

                RaycastHit hitInfo3;
                Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo3, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridCollider"))) {
                    currentObject.position = new Vector3(Mathf.Round(Mathf.Clamp(hitInfo3.point.x, -(GridManager.Instance.gridSizeX / 2f), (GridManager.Instance.gridSizeX / 2f) - 1f)) + .5f, 0f, Mathf.Round(Mathf.Clamp(hitInfo3.point.z, -(GridManager.Instance.gridSizeZ / 2f), (GridManager.Instance.gridSizeZ / 2f) - 1f)) + .5f);
                }
            }
			else if (!EventSystem.current.IsPointerOverGameObject ())
			{
				if (HitRotateCirlce)
				{
					RaycastHit hitInfo5;
					if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo5, Mathf.Infinity, 1 << LayerMask.NameToLayer ("GridCollider")))
					{
						float angle = SignedAngleBetween (RotateCircleHitPreviousPoint - currentObject.position, hitInfo5.point - currentObject.position, Vector3.up);
						RotateCirclesRenderer.Instance.SetDegrees (-angle);
						RotateCircleHitPreviousPoint = hitInfo5.point;
					}
				}
                // need to select the object that we are hovering over first
				else if (currentObject)
				{
                    Debug.Log("Current");
					RaycastHit hitInfo3;
					Ray ray = PlayerCamera.ScreenPointToRay (Input.mousePosition);

					if (Physics.Raycast (ray, out hitInfo3, Mathf.Infinity, 1 << LayerMask.NameToLayer ("GridCollider")))
					{
						currentObject.position = new Vector3 (Mathf.Round (Mathf.Clamp (hitInfo3.point.x, -(GridManager.Instance.gridSizeX / 2f), (GridManager.Instance.gridSizeX / 2f) - 1f)) + .5f, 0f, Mathf.Round (Mathf.Clamp (hitInfo3.point.z, -(GridManager.Instance.gridSizeZ / 2f), (GridManager.Instance.gridSizeZ / 2f) - 1f)) + .5f);
					}
				}
			}

		}

		if (Input.GetMouseButtonUp(0))
		{
			if (HitRotateCirlce)
				HitRotateCirlce = false;
		}
	}
		

	float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
	{
		float angle = Vector3.Angle(a,b);
		float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

		float signed_angle = angle * sign;

		return signed_angle;
	}

	public void SetObject (string objectName)
	{
        /*Debug.Log("Made Object");
		GameObject newObject = Instantiate(Resources.Load ("ModelPrefabs/" + objectName)) as GameObject;
		newObject.transform.position = Vector3.zero;
		currentObject = newObject.transform;

		SelectedObjectCircleRenderer.Instance.SetSelectedObject (currentObject);*/

        /*AssetBundleRequest request = DataManager.modelBundle.LoadAssetAsync(objectName, typeof(GameObject));
        yield return request;
        GameObject newObject = (GameObject)request.asset;
        newObject.transform.position = Vector3.zero;
        currentObject = newObject.transform;

        SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);*/

		isObjSelected = true;

        StartCoroutine(LoadObject(objectName));
	}

    IEnumerator LoadObject(string objectName) {
        // Load and retrieve the AssetBundle
        AssetBundle[] bundles = DataManager.modelBundles.ToArray();
        AssetBundle bundle = new AssetBundle();

        for (int i = 0; i < bundles.Length; i++) {
            if(bundles[i].Contains(objectName)) {
                bundle = bundles[i];
                break;
            }
        }

        // Load the object asynchronously
        AssetBundleRequest request = bundle.LoadAssetAsync(objectName, typeof(GameObject));

        // Wait for completion
        yield return request;

        // Get the reference to the loaded object
        GameObject newObject = Instantiate(request.asset) as GameObject;
        newObject.transform.position = Vector3.zero;
        newObject.AddComponent<BoxCollider>();
        newObject.GetComponent<BoxCollider>().center = Vector3.zero;
        newObject.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
        newObject.GetComponent<BoxCollider>().isTrigger = true;
        newObject.AddComponent<SelectedObjectCollision>();
        newObject.layer = LayerMask.NameToLayer("DesignObject");
        newObject.tag = "Models";
        currentObject = newObject.transform;
        SelectedObjectCircleRenderer.Instance.SetSelectedObject(currentObject);
    }

    public static IEnumerator LoadObjectAtPositionAndRotation(string objectName, Vector3 position, Quaternion rotation) {
        // Load and retrieve the AssetBundle
        AssetBundle[] bundles = DataManager.modelBundles.ToArray();
        AssetBundle bundle = new AssetBundle();

        for (int i = 0; i < bundles.Length; i++) {
            if (bundles[i].Contains(objectName)) {
                bundle = bundles[i];
                break;
            }
        }

        // Load the object asynchronously
        AssetBundleRequest request = bundle.LoadAssetAsync(objectName, typeof(GameObject));

        // Wait for completion
        yield return request;

        // Get the reference to the loaded object
        GameObject newObject = Instantiate(request.asset) as GameObject;
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        newObject.AddComponent<BoxCollider>();
        newObject.GetComponent<BoxCollider>().center = Vector3.zero;
        newObject.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
        newObject.GetComponent<BoxCollider>().isTrigger = true;
        newObject.AddComponent<SelectedObjectCollision>();
        newObject.layer = LayerMask.NameToLayer("DesignObject");
        newObject.tag = "Models";
    }

    public void DeleteCurrentObject ()
	{
		DestroyImmediate (currentObject.gameObject);
		ObjectWorldPanel.Instance.SetTarget (null);
	}

	public void SetSelectableToNull ()
	{
		isObjSelected = false;
		if (currentObject)
		{
			IUISelectable selectable = currentObject.GetComponent<IUISelectable> ();

			if (selectable != null)
				selectable.Select (false);

			currentObject = null;
		}

		MoveableUISelected = !hasHitUI;
		ObjectWorldPanel.Instance.SetTarget (null);
		SelectedObjectCircleRenderer.Instance.SetSelectedObject (null);
	}
}
