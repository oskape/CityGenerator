using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buildings : Build {

	private GameObject block;
	private GameObject[] roofBlocks;
	private GameObject window;
	private GameObject door;
	private GameObject intersection;

	public void InitWindow(Vector2 scale, Vector2 textureScale, Texture texture)
	{
		window = QuadMesh (scale, texture, textureScale, "Window");
		window.SetActive (false);

	}

	public void InitDoor(Vector2 scale, Vector2 textureScale, Texture texture)
	{
		door = QuadMesh (scale, texture, textureScale, "Door");
		door.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		
	}

	public GameObject SuburbanHouse(Vector2 plotScale, Texture roofTex, Texture windowTex, Texture doorTex, Texture buildingTex)
	{
		Vector3 buildingScale;
		buildingScale.x = Random.Range(minHouseScale.x, plotScale.x);
		buildingScale.y = Random.Range(minHouseScale.y, maxHouseScale.y);
		buildingScale.z = Random.Range(minHouseScale.z, plotScale.y);

		Vector3 offset = new Vector3 (Random.Range (0, plotScale.x - buildingScale.x), 0, Random.Range (0, plotScale.y - buildingScale.z));

		GameObject building = BoxMesh(buildingScale, buildingTex, 0.3f, "Building");
		building.transform.Translate (offset);
		GameObject[] roof = RightAngleRoof (0, buildingScale, new Vector3 (0, 0, 0), roofTex, buildingTex);
		for (int i = 0; i < roof.Length; i++) {
			roof[i].transform.SetParent (building.transform, false);
		}

		// doors, need path (single door with uv?)
		Vector3 doorRotation = new Vector3 (-90.0f, 0.0f, 0.0f);
		Vector3 doorOffset = new Vector3 ((int)Random.Range (0.0f, buildingScale.x-doorScale.x), 0.0f, -0.02f);
		if (offset.z < plotScale.y - (offset.z + buildingScale.z)) {
			doorOffset.x += doorScale.x;
			doorOffset.z += buildingScale.z + 0.04f;
			doorRotation.y += 180.0f;
		}
		GameObject thisDoor = (GameObject)Instantiate (door, doorOffset, Quaternion.Euler (doorRotation));
		thisDoor.SetActive (true);
//		GameObject thisDoor = QuadMesh (doorScale, doorTex, new Vector2(1.0f, 1.0f), "Building");
//		thisDoor.transform.Translate (doorOffset);
//		thisDoor.transform.Rotate (doorRotation);
		thisDoor.transform.SetParent(building.transform, false);

		// garages
		if (offset.x > minHouseScale.x) {
			Vector3 garageOffset = new Vector3(-minHouseScale.x, 0.0f, 0.0f);
			GameObject garage = Garage (buildingScale, buildingTex, roofTex);
			garage.transform.SetParent (building.transform, false);
			garage.transform.Translate (garageOffset);
		}
		else if (plotScale.x - (offset.x + buildingScale.x) > minHouseScale.x) {
			Vector3 garageOffset = new Vector3(buildingScale.x, 0.0f, 0.0f);
			GameObject garage = Garage (buildingScale, buildingTex, roofTex);
			garage.transform.SetParent (building.transform, false);
			garage.transform.Translate (garageOffset);
		}

		if (windowTex != null)
		{
			GameObject[] windows = Windows (buildingScale, windowTex);
			for (int i = 0; i < windows.Length; i++) {
				windows [i].transform.SetParent (building.transform, false);
			}
		}

		return building;
	}

	private GameObject Garage(Vector3 buildingScale, Texture buildingTexture, Texture roofTexture)
	{
		Vector3 garageScale = new Vector3 (minHouseScale.x, 3.0f, 0.5f * buildingScale.z);
		GameObject garage = BoxMesh(garageScale, buildingTexture, 0.3f, "Building");
		GameObject[] roof = RightAngleRoof (0, garageScale, new Vector3 (0, 0, 0), roofTexture, buildingTexture);
		for (int i = 0; i < roof.Length; i++) {
			roof[i].transform.SetParent (garage.transform, false);
		}

		return garage;
	}

	public GameObject[] TownHouse(Vector2 plotScale, Texture buildingTex, Texture roofTex, Texture windowTex, Texture doorTex)
	{
		List<GameObject> buildings = new List<GameObject> ();
		Vector3 offset = new Vector3(Random.Range(0, plotScale.x - minBlockScale.x), 0, Random.Range(0, plotScale.y - minBlockScale.y));
		Vector3 scale;

		while (offset.x + minBlockScale.x <= plotScale.x) {
			scale.x = (int)Random.Range (minBlockScale.x, plotScale.x - offset.x);
			while (offset.z + minBlockScale.z <= plotScale.y) {
				scale.z = (int)Random.Range (minBlockScale.z, plotScale.y - offset.z);
				scale.y = (int)Random.Range (minBlockScale.y, maxBlockScale.y);
				Vector3 thisScale = scale;
				Vector3 thisOffset = offset;
				bool goneCircular = false;

				while (thisOffset.y + minBlockScale.y <= maxBlockScale.y) {
					
					if (goneCircular || (thisScale.x == thisScale.z && thisScale.x >= 10.0f)) {
						goneCircular = true;
						GameObject building = ConeMesh (true, thisScale, buildingTex, "Building");
						building.transform.Translate (0.5f * thisScale.x, 0, 0.5f * thisScale.z);
						building.transform.Translate (thisOffset);
						buildings.Add (building);

						Vector3 roofScale = new Vector3 (thisScale.x, 0.2f * thisScale.y, 0.0f);
						GameObject roof = ConeMesh (false, roofScale, roofTex, "Building");
						roof.transform.SetParent (building.transform, false);
						roof.transform.Translate (0.0f, thisScale.y, 0.0f);

						//break; or maths???
					} else {
						GameObject building = BoxMesh (thisScale, buildingTex, 0.3f, "Building");
						building.transform.Translate (thisOffset);
						buildings.Add (building);

						// laggyyyyy
						if (windowTex != null)
						{
							GameObject[] windows = Windows (thisScale, windowTex);
							for (int i = 0; i < windows.Length; i++) {
								windows [i].transform.SetParent (building.transform, false);
							}
						}

						GameObject[] roof = RightAngleRoof (0, thisScale, new Vector3(0,0,0), roofTex, buildingTex);
						for (int i = 0; i < roof.Length; i++) {
							roof [i].transform.SetParent (building.transform, false);
						}
					}

					thisOffset.y += thisScale.y;

					Vector3 oldScale = thisScale;
					thisScale = new Vector3 ((int)Random.Range (minBlockScale.x, thisScale.x), (int)Random.Range (minBlockScale.y, maxBlockScale.y), (int)Random.Range (minBlockScale.z, thisScale.z));
					if (goneCircular)
						thisScale.x = thisScale.z;
					thisOffset.x += Random.Range (0, oldScale.x - thisScale.x);
					thisOffset.z += Random.Range (0, oldScale.z - thisScale.z);
				}
				offset.z += scale.z;
			}
			offset.x += scale.x;
		}

		return buildings.ToArray();
	}

	// hmmmm
	public GameObject[] WindowColumn(float windowSpace, Vector3 windowScale, float buildingHeight, Texture windowTex)
	{
		List<GameObject> windows = new List<GameObject> ();
		Vector2 texScale = new Vector2(1.0f, 1.0f);
		Vector3 windowPosition = new Vector3(0, windowScale.y, 0);

		while (windowPosition.y <= buildingHeight - 2.0f * windowScale.y) {
			if (Random.Range (0, 2) == 0) {
				GameObject thisWindow = (GameObject)Instantiate (window, windowPosition, Quaternion.identity);
//				windows.Add ((GameObject)Instantiate (window, windowPosition, Quaternion.identity));
				thisWindow.SetActive (true);
//				GameObject thisWindow = QuadMesh(windowScale, windowTex, texScale, "Building");
//				thisWindow.transform.Translate (windowPosition);
				windows.Add(thisWindow);
			}
			windowPosition.y += windowSpace;
		}

		return windows.ToArray ();
	}

	private GameObject[] Windows(Vector3 buildingScale, Texture windowTex)
	{
		List<GameObject> windows = new List<GameObject> ();

		Vector2 windowScale = new Vector3(1.0f, 1.0f);
		float floorHeight = 3.0f * windowScale.y;

		// south
		Vector3 windowPosition = new Vector3 (windowScale.x, 0.0f, -0.01f);

		while (windowPosition.x <= buildingScale.x-2.0f*windowScale.x)
		{
			GameObject[] column = WindowColumn (floorHeight, windowScale, buildingScale.y, windowTex);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, 0.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.x += floorHeight; // Dedicated value?
		}

		// north (double corner buffer for rotation, opposite corner buffer reduced)
		windowPosition = new Vector3 (2.0f*windowScale.x, 0.0f, buildingScale.z + 0.01f);

		while (windowPosition.x <= buildingScale.x-windowScale.x)
		{
			GameObject[] column = WindowColumn (floorHeight, windowScale, buildingScale.y, windowTex);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, 180.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.x += floorHeight; // Dedicated value?
		}

		// east (double corner buffer for rotation, opposite corner buffer reduced)
		windowPosition = new Vector3 (-0.01f, 0.0f, 2.0f*windowScale.x);

		while (windowPosition.z <= buildingScale.z-windowScale.x)
		{
			GameObject[] column = WindowColumn (floorHeight, windowScale, buildingScale.y, windowTex);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, 90.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.z += floorHeight; // Dedicated value?
		}

		// west
		windowPosition = new Vector3 (buildingScale.x+0.01f, 0.0f, windowScale.x);

		while (windowPosition.z <= buildingScale.z-2.0f*windowScale.x)
		{
			GameObject[] column = WindowColumn (floorHeight, windowScale, buildingScale.y, windowTex);
			for (int i = 0; i < column.Length; i++) {
				column [i].transform.Translate (windowPosition);
				column [i].transform.Rotate (new Vector3 (-90.0f, -90.0f, 0.0f));
//				column [i].SetActive (true);
				windows.Add (column [i]);
			}
			windowPosition.z += floorHeight; // Dedicated value?
		}

		return windows.ToArray();
	}

	// Fix block array
	private GameObject[] RightAngleRoof(int roofNum, Vector3 houseScale, Vector3 position, Texture roofTex, Texture buildingTex)
	{
		GameObject[] roofBlocks;
		roofBlocks = new GameObject[5];

		float roofHeight = (int)Random.Range (0.0f, 0.5f * houseScale.y);
		float angle = Mathf.Atan2 (roofHeight, 0.5f*houseScale.z);

		Vector3 roofScale;
		roofScale.x = houseScale.x;
		roofScale.y = 0.5f*houseScale.z / Mathf.Cos (angle);
		roofScale.z = 0.1f;

		if (Random.Range (0, 2) == 0) {
			Vector3 roofPosition1 = position;
			roofPosition1.x += 0.5f * roofScale.x;
			roofPosition1.y += houseScale.y + 0.5f * roofHeight;
			roofPosition1.z += 0.25f * houseScale.z;

			Vector2 texScale = new Vector2(roofScale.x, roofScale.y);
			Vector3 rotationDegrees = new Vector3((90.0f - angle * Mathf.Rad2Deg), 0.0f, 0.0f);
			roofBlocks [0] = SetupPrimitive (PrimitiveType.Quad, rotationDegrees, roofScale, roofPosition1, roofTex, texScale, "Building");

			Vector3 roofPosition2 = roofPosition1;
			roofPosition2.z += 0.5f*houseScale.z;
			rotationDegrees.y += 180.0f;
			roofBlocks [1] = SetupPrimitive (PrimitiveType.Quad, rotationDegrees, roofScale, roofPosition2, roofTex, texScale, "Building");

			texScale = new Vector2(1.0f, 1.0f);
			Vector3[] vertPos = new Vector3[] {
				new Vector3 (position.x, position.y + houseScale.y, position.z),
				new Vector3 (position.x, position.y + houseScale.y + roofHeight, position.z + 0.5f*houseScale.z),
				new Vector3 (position.x, position.y + houseScale.y, position.z + houseScale.z)};
			roofBlocks [2] = TriangleMesh (vertPos, true, buildingTex, texScale);

			for (int i = 0; i < vertPos.Length; i++) {
				vertPos [i].x += houseScale.x;
			}
			roofBlocks [3] = TriangleMesh (vertPos, false, buildingTex, texScale);
		}

		else {
			Vector3 cornerSW = new Vector3(position.x, position.y+houseScale.y, position.z);
			Vector3 cornerSE = new Vector3(position.x+houseScale.x, position.y+houseScale.y, position.z);
			Vector3 cornerNW = new Vector3(position.x, position.y+houseScale.y, position.z+houseScale.z);
			Vector3 cornerNE = new Vector3(position.x+houseScale.x, position.y+houseScale.y, position.z+houseScale.z);
			Vector3 top = new Vector3 (position.x+0.5f*houseScale.x, position.y+houseScale.y+roofHeight, position.z+0.5f*houseScale.z);

			Vector2 texScaleWide = new Vector2 (houseScale.x, roofScale.y);
			Vector2 texScaleLong = new Vector2 (houseScale.z, Mathf.Sqrt(Mathf.Pow(roofHeight, 2) + Mathf.Pow(houseScale.x*0.5f, 2)));

			Vector3[] vertPos = new Vector3[] { cornerSW, top, cornerSE };
			roofBlocks[0] = TriangleMesh (vertPos, false, roofTex, texScaleWide);

			vertPos = new Vector3[] { cornerSE, top, cornerNE };
			roofBlocks[1] = TriangleMesh (vertPos, false, roofTex, texScaleLong);

			vertPos = new Vector3[] { cornerNE, top, cornerNW };
			roofBlocks[2] = TriangleMesh (vertPos, false, roofTex, texScaleWide);

			vertPos = new Vector3[] { cornerNW, top, cornerSW };
			roofBlocks[3] = TriangleMesh (vertPos, false, roofTex, texScaleLong);
		}

		Vector3 chimneyScale = new Vector3 (1.0f, roofHeight, 1.0f);
		if (roofHeight < 1.0f)
			chimneyScale.y = 1.0f;
		Vector2 chimneyTexScale = new Vector2 (1.0f, chimneyScale.y);
		Vector3 chimneyPos = new Vector3
			(position.x + 0.01f + Random.Range (0.5f*chimneyScale.x, houseScale.x-0.5f*chimneyScale.x),
				position.y + houseScale.y + 0.5f*chimneyScale.y,
				position.z + 0.01f + Random.Range(0.5f*chimneyScale.z, houseScale.z-0.5f*chimneyScale.z));
		roofBlocks [4] = SetupPrimitive (PrimitiveType.Cube, new Vector3 (0.0f, 0.0f, 0.0f), chimneyScale, chimneyPos, roofTex, chimneyTexScale, "Building");

		return roofBlocks;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
