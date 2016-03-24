using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buildings : Build {

	private GameObject window;
	private GameObject door;
	private GameObject garage;

	private Vector3 minBuildingScale;
	private Vector3 maxBuildingScale;

	private int windowOdds;
	private Vector2 windowScale;
	private Vector2 windowSpacing;
	
	private Vector2 doorScale;

	private Vector3 garageScale;

	private Texture plotTexture;
	private Texture[] buildingTextures;
	private Texture roofTexture;

	private bool stacking;

	public void InitPlots(Texture aPlotTexture)
	{
		plotTexture = aPlotTexture;
	}

	public void InitBuildings(bool aStacking, Vector3 aMinBuildingScale, Vector3 aMaxBuildingScale, Vector3 aGarageScale, Texture[] aBuildingTextures, Texture aRoofTexture)
	{
		stacking = aStacking;

		minBuildingScale = aMinBuildingScale;
		maxBuildingScale = aMaxBuildingScale;

		buildingTextures = new Texture[aBuildingTextures.Length];
		for (int i = 0; i < aBuildingTextures.Length; i++) {
			buildingTextures[i] = aBuildingTextures[i];
		}
		
		roofTexture = aRoofTexture;

		garageScale = aGarageScale;
		garage = BoxMesh(garageScale, buildingTextures[0], 0.3f, "Building");
		garage.SetActive (false);
	}

	public void InitWindows(int aWindowOdds, Vector2 aWindowScale, Vector2 aWindowSpacing, Texture aWindowTexture)
	{
		windowOdds = aWindowOdds;
		windowScale = aWindowScale;
		windowSpacing = aWindowSpacing;

		window = QuadMesh (windowScale, aWindowTexture, new Vector2(1.0f, 1.0f), "Window");
		window.SetActive (false);
	}

	public void InitDoors(Vector2 aDoorScale, Texture aDoorTexture)
	{
		doorScale = aDoorScale;
		door = QuadMesh (aDoorScale, aDoorTexture, new Vector2(1.0f, 1.0f), "Door");
		door.SetActive (false);
	}

	public GameObject SetupPlot(Vector2 plotScale)
	{
		Vector3 buildingScale = new Vector3 ((int)Random.Range (minBuildingScale.x, maxBuildingScale.x), (int)Random.Range (minBuildingScale.y, maxBuildingScale.y), (int)Random.Range (minBuildingScale.z, maxBuildingScale.z));
		buildingScale.x = Mathf.Clamp (buildingScale.x, minBuildingScale.x, plotScale.x);
		buildingScale.z = Mathf.Clamp (buildingScale.z, minBuildingScale.z, plotScale.y);

		Vector3 buildingOffset = new Vector3 (Random.Range (0.0f, plotScale.x - buildingScale.x), 0.0f, Random.Range (0.0f, plotScale.y - buildingScale.z));

		if (stacking)
			return BlockPlot (plotScale, buildingScale, buildingOffset);
		else
			return HousePlot (plotScale, buildingScale, buildingOffset);
		
	}

	private GameObject HousePlot(Vector3 plotScale, Vector3 buildingScale, Vector3 buildingOffset)
	{
		GameObject plot = QuadMesh (plotScale, plotTexture, 0.05f * plotScale, "Road");

		GameObject building = BoxMesh(buildingScale, buildingTextures[0], 0.3f, "Building");
		building.transform.Translate (buildingOffset);
		building.transform.SetParent (plot.transform, false);

		GameObject[] roof = RightAngleRoof (buildingScale);
		for (int i = 0; i < roof.Length; i++) {
			roof[i].transform.SetParent (building.transform, false);
		}

		// doors, need path (single door with uv?) / doorScale still here?
		bool frontDoor = true;
		Vector3 doorRotation = new Vector3 (-90.0f, 0.0f, 0.0f);
		Vector3 doorOffset = new Vector3 ((int)Random.Range (0.0f, buildingScale.x-doorScale.x), 0.0f, -0.02f);
		if (buildingOffset.z < plotScale.y - (buildingOffset.z + buildingScale.z)) {
			doorOffset.x += doorScale.x;
			doorOffset.z += buildingScale.z + 0.04f;
			doorRotation.y += 180.0f;
			frontDoor = false;
		}
		GameObject thisDoor = (GameObject)Instantiate (door, doorOffset, Quaternion.Euler (doorRotation));
		thisDoor.SetActive (true);
		//		GameObject thisDoor = QuadMesh (doorScale, doorTex, new Vector2(1.0f, 1.0f), "Building");
		//		thisDoor.transform.Translate (doorOffset);
		//		thisDoor.transform.Rotate (doorRotation);
		thisDoor.transform.SetParent(building.transform, false);

		// garages
		if (buildingOffset.x > garageScale.x) {
			Vector3 garageOffset = new Vector3(-garageScale.x, 0.0f, 0.0f);
			if (frontDoor) {
				garageOffset.z += buildingScale.z - garageScale.z;
			}
			GameObject thisGarage = Garage (garageOffset);
			thisGarage.transform.SetParent (building.transform, false);
		}
		else if (plotScale.x - (buildingOffset.x + buildingScale.x) > garageScale.x) {
			Vector3 garageOffset = new Vector3(buildingScale.x, 0.0f, 0.0f);
			if (frontDoor) {
				garageOffset.z += buildingScale.z - garageScale.z;
			}
			GameObject thisGarage = Garage (garageOffset);
			thisGarage.transform.SetParent (building.transform, false);
		}

		// check if initialised?
		GameObject[] windows = Windows (buildingScale);
		for (int i = 0; i < windows.Length; i++) {
			windows [i].transform.SetParent (building.transform, false);
		}

		return plot;
	}

	private GameObject Garage(Vector3 offset)
	{
		GameObject thisGarage = Instantiate (garage);
		thisGarage.SetActive (true);
		thisGarage.transform.Translate (offset);

		GameObject[] roof = RightAngleRoof (garageScale);
		for (int i = 0; i < roof.Length; i++) {
			roof[i].transform.SetParent (thisGarage.transform, false);
		}

		return thisGarage;
	}

	public GameObject BlockPlot(Vector3 plotScale, Vector3 buildingScale, Vector3 buildingOffset)
	{
		GameObject plot = QuadMesh (plotScale, plotTexture, 0.05f * plotScale, "Road");

		while (buildingOffset.x + buildingScale.x <= plotScale.x) {
			while (buildingOffset.z + buildingScale.z <= plotScale.y) {
				Vector3 thisScale = buildingScale;
				Vector3 thisOffset = buildingOffset;
				bool goneCircular = false;

				while (thisOffset.y + thisScale.y <= maxBuildingScale.y) {

					if (goneCircular || (thisScale.x == thisScale.z && thisScale.x >= 10.0f)) {
						goneCircular = true;
						GameObject building = ConeMesh (true, thisScale, buildingTextures[1], 1.0f, "Building");
						building.transform.Translate (0.5f * thisScale.x, 0, 0.5f * thisScale.z);
						building.transform.Translate (thisOffset);
						building.transform.SetParent (plot.transform, false);

						Vector3 roofScale = new Vector3 (thisScale.x, 0.2f * thisScale.y, 0.0f);
						GameObject roof = ConeMesh (false, roofScale, roofTexture, 1.0f, "Building");
						roof.transform.SetParent (building.transform, false);
						roof.transform.Translate (0.0f, thisScale.y, 0.0f);

						//break; or maths???
					} else {
						GameObject building = BoxMesh (thisScale, buildingTextures[0], 0.3f, "Building");
						building.transform.Translate (thisOffset);
						building.transform.SetParent (plot.transform, false);

						// check if initialised?
						GameObject[] windows = Windows (thisScale);
						for (int i = 0; i < windows.Length; i++) {
							windows [i].transform.SetParent (building.transform, false);
						}

						GameObject[] roof = RightAngleRoof (thisScale);
						for (int i = 0; i < roof.Length; i++) {
							roof [i].transform.SetParent (building.transform, false);
						}
					}

					thisOffset.y += thisScale.y;

					Vector3 oldScale = thisScale;
					thisScale = new Vector3 ((int)Random.Range (minBuildingScale.x, thisScale.x), (int)Random.Range (minBuildingScale.y, maxBuildingScale.y), (int)Random.Range (minBuildingScale.z, thisScale.z));
					if (goneCircular)
						thisScale.x = thisScale.z;
					thisOffset.x += Random.Range (0, oldScale.x - thisScale.x);
					thisOffset.z += Random.Range (0, oldScale.z - thisScale.z);
				}
				buildingOffset.z += buildingScale.z;
			}
			buildingOffset.x += buildingScale.x;
		}

		return plot;
	}
		
	public GameObject[] WindowColumn(float buildingHeight)
	{
		List<GameObject> windows = new List<GameObject> ();
		Vector3 windowPosition = new Vector3(0, windowScale.y, 0);

		while (windowPosition.y <= buildingHeight - 2.0f * windowScale.y) {
			if (Random.Range (0, windowOdds) == 0) {
				GameObject thisWindow = (GameObject)Instantiate (window, windowPosition, Quaternion.identity);
				thisWindow.SetActive (true);
//				GameObject thisWindow = QuadMesh(windowScale, windowTex, texScale, "Building");
//				thisWindow.transform.Translate (windowPosition);
				windows.Add(thisWindow);
			}
			windowPosition.y += windowSpacing.y;
		}

		return windows.ToArray ();
	}

	// give class more of the variables
	private GameObject[] Windows(Vector3 buildingScale)
	{
		List<GameObject> windows = new List<GameObject> ();

		// south
		Vector3 windowPosition = new Vector3 (windowScale.x, 0.0f, -0.01f);

		while (windowPosition.x <= buildingScale.x-2.0f*windowScale.x)
		{
			GameObject[] column = WindowColumn (buildingScale.y);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, 0.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.x += windowSpacing.x;
		}

		// north (double corner buffer for rotation, opposite corner buffer reduced)
		windowPosition = new Vector3 (2.0f*windowScale.x, 0.0f, buildingScale.z + 0.01f);

		while (windowPosition.x <= buildingScale.x-windowScale.x)
		{
			GameObject[] column = WindowColumn (buildingScale.y);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, 180.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.x += windowSpacing.x;
		}

		// east (double corner buffer for rotation, opposite corner buffer reduced)
		windowPosition = new Vector3 (-0.01f, 0.0f, 2.0f*windowScale.x);

		while (windowPosition.z <= buildingScale.z-windowScale.x)
		{
			GameObject[] column = WindowColumn (buildingScale.y);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, 90.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.z += windowSpacing.x;
		}

		// west
		windowPosition = new Vector3 (buildingScale.x+0.01f, 0.0f, windowScale.x);

		while (windowPosition.z <= buildingScale.z-2.0f*windowScale.x)
		{
			GameObject[] column = WindowColumn (buildingScale.y);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, -90.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.z += windowSpacing.x;
		}

		return windows.ToArray();
	}
		
	private GameObject[] RightAngleRoof(Vector3 houseScale)
	{

		GameObject[] roofBlocks;
		roofBlocks = new GameObject[5];

		float roofHeight = (int)Random.Range (0.0f, 0.5f * houseScale.y);
		float angle = Mathf.Atan2 (roofHeight, 0.5f*houseScale.z);

		Vector2 roofScale = new Vector2 (houseScale.x, 0.5f * houseScale.z / Mathf.Cos (angle));

		if (Random.Range (0, 2) == 0) {

			Vector3 slopeRotation = new Vector3(-(angle * Mathf.Rad2Deg), 0.0f, 0.0f);
			roofBlocks [0] = QuadMesh (roofScale, roofTexture, roofScale, "Building");//SetupPrimitive (PrimitiveType.Quad, rotationDegrees, roofScale, roofPosition1, roofTexture, texScale, "Building");
			roofBlocks [0].transform.Translate (0.0f, houseScale.y, 0.0f);
			roofBlocks [0].transform.Rotate (slopeRotation);

			slopeRotation.y += 180.0f;
			roofBlocks [1] = QuadMesh (roofScale, roofTexture, roofScale, "Building");//SetupPrimitive (PrimitiveType.Quad, rotationDegrees, roofScale, roofPosition2, roofTexture, texScale, "Building");
			roofBlocks [1].transform.Translate (houseScale.x, houseScale.y, houseScale.z);
			roofBlocks [1].transform.Rotate (slopeRotation);

			Vector3[] vertPos = new Vector3[] {
				new Vector3 (0.0f, 0.0f, 0.0f),
				new Vector3 (0.0f, roofHeight, 0.5f*houseScale.z),
				new Vector3 (0.0f, 0.0f, houseScale.z)};
			roofBlocks [2] = TriangleMesh (vertPos, true, buildingTextures[0], new Vector2 (0.3f * houseScale.z, 0.3f * roofHeight));
			roofBlocks [2].transform.Translate (0.0f, houseScale.y, 0.0f);

			roofBlocks [3] = TriangleMesh (vertPos, false, buildingTextures[0], new Vector2 (0.3f * houseScale.z, 0.3f * roofHeight));
			roofBlocks [3].transform.Translate (houseScale.x, houseScale.y, 0.0f);
		}

		else {
			Vector3 cornerSW = new Vector3 (0.0f, 0.0f, 0.0f);
			Vector3 cornerSE = new Vector3 (houseScale.x, 0.0f, 0.0f);
			Vector3 cornerNW = new Vector3 (0.0f, 0.0f, houseScale.z);
			Vector3 cornerNE = new Vector3 (houseScale.x, 0.0f, houseScale.z);
			Vector3 top = new Vector3 (0.5f * houseScale.x, roofHeight, 0.5f * houseScale.z);

			Vector2 texScaleWide = new Vector2 (houseScale.x, roofScale.y);
			Vector2 texScaleLong = new Vector2 (houseScale.z, Mathf.Sqrt (Mathf.Pow (roofHeight, 2) + Mathf.Pow (houseScale.x * 0.5f, 2)));

			Vector3[] vertPos = new Vector3[] { cornerSW, top, cornerSE };
			roofBlocks [0] = TriangleMesh (vertPos, false, roofTexture, texScaleWide);
			roofBlocks [0].transform.Translate (0.0f, houseScale.y, 0.0f);

			vertPos = new Vector3[] { cornerSE, top, cornerNE };
			roofBlocks [1] = TriangleMesh (vertPos, false, roofTexture, texScaleLong);
			roofBlocks [1].transform.Translate (0.0f, houseScale.y, 0.0f);

			vertPos = new Vector3[] { cornerNE, top, cornerNW };
			roofBlocks [2] = TriangleMesh (vertPos, false, roofTexture, texScaleWide);
			roofBlocks [2].transform.Translate (0.0f, houseScale.y, 0.0f);

			vertPos = new Vector3[] { cornerNW, top, cornerSW };
			roofBlocks [3] = TriangleMesh (vertPos, false, roofTexture, texScaleLong);
			roofBlocks [3].transform.Translate (0.0f, houseScale.y, 0.0f);
		}

		Vector3 chimneyScale = new Vector3 (1.0f, roofHeight, 1.0f);
		if (roofHeight < 1.0f)
			chimneyScale.y = 1.0f;
		Vector2 chimneyTexScale = new Vector2 (1.0f, chimneyScale.y);
		Vector3 chimneyPos = new Vector3 (0.01f + Random.Range (0.5f * chimneyScale.x, houseScale.x - 0.5f * chimneyScale.x),
			                     houseScale.y + 0.5f * chimneyScale.y,
			                     0.01f + Random.Range (0.5f * chimneyScale.z, houseScale.z - 0.5f * chimneyScale.z));
		roofBlocks [4] = SetupPrimitive (PrimitiveType.Cube, new Vector3 (0.0f, 0.0f, 0.0f), chimneyScale, chimneyPos, roofTexture, chimneyTexScale, "Building");

		return roofBlocks;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
