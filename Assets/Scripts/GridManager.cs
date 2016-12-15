using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	protected static GridManager _instance;

	public static GridManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(GridManager)) as GridManager;

				if(_instance == null)
				{
					Debug.LogError("Warning: there should always be an instance of GridManager in the scene.");
				}
			}
			return _instance;
		}
	}

	public int gridSizeX;
	public int gridSizeY;
	public int gridSizeZ;

	private float StepSize = 1f;

	private float startX = 0f;
	private float startY = 0f;
	private float startZ = 0f;

	public Material lineMaterial;

	public Button PlusGridSizeButton;
	public Button MinusGridSizeButton;

	void Start ()
	{
		PlusGridSizeButton.onClick.AddListener (PlusGridSize);
		MinusGridSizeButton.onClick.AddListener (MinusGridSize);

		startX -= gridSizeX / 2f;
		startZ -= gridSizeZ / 2f;
		startY -= gridSizeY / 2f;
	}

	void OnPostRender() 
	{        
		lineMaterial.SetPass( 0 );

		GL.Begin( GL.LINES );

		for(float j = 0; j <= gridSizeY; j += StepSize)
		{
			//X axis lines
			for(float i = 0; i <= gridSizeZ; i += StepSize)
			{
				GL.Vertex3( startX, startY + j, startZ + i);
				GL.Vertex3( startX + gridSizeX, startY + j, startZ + i);
			}

			//Z axis lines
			for(float i = 0; i <= gridSizeX; i += StepSize)
			{
				GL.Vertex3( startX + i, startY + j, startZ);
				GL.Vertex3( startX + i, startY + j, startZ + gridSizeZ);
			}
		}

		for(float i = 0; i <= gridSizeZ; i += StepSize)
		{
			for(float k = 0; k <= gridSizeX; k += StepSize)
			{
				GL.Vertex3( startX + k, startY, startZ + i);
				GL.Vertex3( startX + k, startY + gridSizeY, startZ + i);
			}
		}

		GL.End();
	}

	public void PlusGridSize ()
	{
		ChangeGridSize (10);
	}

	public void MinusGridSize ()
	{
		ChangeGridSize (-10);
	}

	void ChangeGridSize (int sizeChange)
	{
		int gridSize = gridSizeX + sizeChange;

		gridSize = Mathf.Clamp (gridSize, 20, 40);

		gridSizeX = gridSizeZ = gridSize;

		startX = -(float) gridSizeX / 2f;
		startZ = -(float) gridSizeZ / 2f;
		startY = -(float) gridSizeY / 2f;

		CameraPositions.Instance.UpdatePosition ();
	}
}