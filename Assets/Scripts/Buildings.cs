using UnityEngine;
using System.Collections;

public class Buildings : Build {

	private GameObject block;
	private GameObject[] roofBlocks;
	//private GameObject[] windowBlocks;

	public void MakeBuilding(Vector3 scale, Vector3 position, Texture roofTex, Texture windowTex)
	{
		block = new GameObject();

		Vector3 houseScale = scale;
		Vector3 housePosition = position + 0.5f*houseScale;
		Vector3 houseRotation = new Vector3(0.0f, 0.0f, 0.0f);
		Vector2 texScale = new Vector2(1.0f, 1.0f);
		block = SetupPrimitive(PrimitiveType.Cube, houseRotation, houseScale, housePosition, null, texScale, "Building");
		//houseScale.y *= 0.5f;
		//block = SetupPrimitive(PrimitiveType.Cylinder, houseRotation, houseScale, housePosition, null, texScale, "Building");

		if (roofTex != null)
		{
			//roofBlocks = new GameObject[5];
			roofBlocks = RightAngleRoof(0, scale, position, roofTex);
		}

		if (windowTex != null)
		{
			Windows(position, scale, windowTex);
		}
	}

	public void SuburbanHouse(Vector3 plotScale, Vector3 plotPosition, Texture roofTex, Texture windowTex)
	{
		Vector3 buildingScale;
		buildingScale.x = Random.Range(minHouseScale.x, plotScale.x);
		buildingScale.y = plotScale.y;
		buildingScale.z = Random.Range(minHouseScale.z, plotScale.z);

		Vector3 buildingPos = plotPosition;
		buildingPos.x += Random.Range(0, plotScale.x - buildingScale.x);
		buildingPos.z += Random.Range(0, plotScale.z - buildingScale.z);

		MakeBuilding(buildingScale, buildingPos, roofTex, windowTex);

		// doors, need path (single door with uv?)
		if (buildingPos.z - plotPosition.z > plotPosition.z + plotScale.z - (buildingPos.z + buildingScale.z)) {
			Vector3 doorScale = new Vector3 (2.0f, 3.0f, 0.1f);
			Vector3 doorPos = new Vector3 (buildingPos.x + (int)Random.Range (0.5f*doorScale.x, buildingScale.x-0.5f*doorScale.x), buildingPos.y + 0.5f*doorScale.y, buildingPos.z - 0.02f);
			SetupPrimitive (PrimitiveType.Quad, new Vector3 (0.0f, 0.0f, 0.0f), doorScale, doorPos, doorTex, new Vector2 (1.0f, 1.0f), "Building");
		} else {
			Vector3 doorScale = new Vector3 (2.0f, 3.0f, 0.1f);
			Vector3 doorPos = new Vector3 (buildingPos.x + (int)Random.Range (0.5f*doorScale.x, buildingScale.x-0.5f*doorScale.x), buildingPos.y + 0.5f*doorScale.y, buildingPos.z + buildingScale.z + 0.02f);
			SetupPrimitive (PrimitiveType.Quad, new Vector3 (0.0f, 180.0f, 0.0f), doorScale, doorPos, doorTex, new Vector2 (1.0f, 1.0f), "Building");
		}

		// garages
		if (buildingPos.x - plotPosition.x > minHouseScale.x) {
			Vector3 garageScale = new Vector3 (minHouseScale.x, 3.0f, 0.5f * buildingScale.z);
			Vector3 garagePosition = buildingPos;
			garagePosition.x -= garageScale.x;
			//new Building (garageScale, garagePosition, null, null);
			MakeBuilding(garageScale, garagePosition, null, null);
		}
		else if ((plotPosition.x + plotScale.x) - (buildingPos.x + buildingScale.x) > minHouseScale.x) {
			Vector3 garageScale = new Vector3 (minHouseScale.x, 3.0f, 0.5f * buildingScale.z);
			Vector3 garagePosition = buildingPos;
			garagePosition.x += buildingScale.x;
			//new Building (garageScale, garagePosition, null, null);
			MakeBuilding(garageScale, garagePosition, null, null);
		}
	}

	public void TownHouse(Vector3 plotScale, Vector3 plotPosition, Texture roofTex, Texture windowTex)
	{
		Vector3 buildingScale;// = plotScale;
		buildingScale.x = Random.Range(minBlockScale.x, plotScale.x);
		buildingScale.y = plotScale.y;
		buildingScale.z = Random.Range(minBlockScale.z, plotScale.z);

		Vector3 buildingPos = plotPosition;
		buildingPos.x += Random.Range(0, plotScale.x - buildingScale.x);
		buildingPos.z += Random.Range(0, plotScale.z - buildingScale.z);

		//Vector3 thisScale = new Vector3 (Random.Range(minBlockScale.x, buildingScale.x), (int)Random.Range (minBlockScale.y, buildingScale.y), Random.Range(minBuildingScale.z, buildingScale.z));
		Vector3 thisPos = buildingPos;
		while (thisPos.x < plotPosition.x+plotScale.x - buildingScale.x) {
			while (thisPos.z < plotPosition.z+plotScale.z - buildingScale.z) {
				Texture hasRoof;
				if (Random.Range (0, 10) == 0)
					hasRoof = roofTex;
				else
					hasRoof = null;
				//new Building (thisScale, thisPos, hasRoof, windowTex);
				MakeBuilding(buildingScale, thisPos, hasRoof, windowTex);

				//vertical stacking
				Vector3 oldScale = buildingScale;
				Vector3 oldPos = thisPos;
				while (oldPos.y + oldScale.y < maxBlockScale.y-minBlockScale.y && oldScale.x > minBlockScale.x+1.0f && oldScale.z > minBlockScale.z+1.0f) {
					Vector3 newScale = new Vector3 (Random.Range (minBlockScale.x, oldScale.x), (int)Random.Range (minBlockScale.y, maxBlockScale.y - oldScale.y), Random.Range (minBlockScale.z, oldScale.z));
					Vector3 newPos = new Vector3 (Random.Range (oldPos.x, oldPos.x + oldScale.x - newScale.x), oldPos.y + oldScale.y, Random.Range (oldPos.z, oldPos.z + oldScale.z - newScale.z));
					//new Building (newScale, newPos, hasRoof, windowTex);
					MakeBuilding(newScale, newPos, hasRoof, windowTex);
					oldScale = newScale;
					oldPos = newPos;
				}

				thisPos.z += buildingScale.z;
				buildingScale.y = (int)Random.Range (minBlockScale.y, maxBlockScale.y);
				buildingScale.z = Random.Range (minBlockScale.z, maxBlockScale.z);
			}
			thisPos.x += buildingScale.x;
			thisPos.z = buildingPos.z;
			buildingScale.x = Random.Range (minBlockScale.x, maxBlockScale.x);
		}
	}

	// hmmmm
	private void WindowColumn(Vector3 windowScale, Vector3 rotation, Vector3 scale, Vector3 position, Texture windowTex)
	{
		Vector2 texScale = new Vector2(1.0f, 1.0f);
		Vector3 windowPosition = position;
		//windowPosition.y += 0.5f * windowScale.y;

		while (windowPosition.y < position.y+scale.y-2.0f*windowScale.y)
		{
			windowPosition.y += 2.0f*windowScale.y;
			if (Random.Range(0, 2) == 0) SetupPrimitive(PrimitiveType.Quad, rotation, windowScale, windowPosition, windowTex, texScale, "Building");
		}
	}

	private void Windows(Vector3 position, Vector3 scale, Texture windowTex)
	{
		//GameObject[] windowBlocks = new GameObject[];

		Vector3 windowScale = new Vector3(1.0f, 1.0f, 0.1f);
		float windowHeight = windowScale.y*2.0f;
		float floorHeight = windowHeight+windowScale.y;

		Vector3 windowPosition = position;
		windowPosition.x += windowHeight + 0.5f*windowScale.x;
		windowPosition.z -= 0.01f;
		Vector3 rotation = new Vector3 (0.0f, 0.0f, 0.0f);

		while (windowPosition.x < position.x+scale.x-windowScale.x)
		{
			WindowColumn (windowScale, rotation, scale, windowPosition, windowTex);
			windowPosition.x += floorHeight; // Dedicated value?
		}

		windowPosition = position;
		windowPosition.x += scale.x + 0.01f;
		windowPosition.z += windowHeight + 0.5f*windowScale.x;
		rotation = new Vector3 (0.0f, -90.0f, 0.0f);

		while (windowPosition.z < position.z+scale.z-windowScale.y)
		{
			WindowColumn (windowScale, rotation, scale, windowPosition, windowTex);
			windowPosition.z += floorHeight; // Dedicated value?
		}

		windowPosition = position;
		windowPosition.x += windowHeight + 0.5f*windowScale.x;
		windowPosition.z += scale.z + 0.01f;
		rotation = new Vector3 (0.0f, 180.0f, 0.0f);

		while (windowPosition.x < position.x+scale.x-windowScale.y)
		{
			WindowColumn (windowScale, rotation, scale, windowPosition, windowTex);
			windowPosition.x += floorHeight; // Dedicated value?
		}

		windowPosition = position;
		windowPosition.x -= 0.01f;
		windowPosition.z += windowHeight + 0.5f*windowScale.x;
		rotation = new Vector3 (0.0f, 90.0f, 0.0f);

		while (windowPosition.z < position.z+scale.z-windowScale.y)
		{
			WindowColumn (windowScale, rotation, scale, windowPosition, windowTex);
			windowPosition.z += floorHeight; // Dedicated value?
		}

		//return windowBlocks;
	}

	// Fix block array
	private GameObject[] RightAngleRoof(int roofNum, Vector3 houseScale, Vector3 position, Texture roofTex)
	{
		GameObject[] roofBlocks;
		roofBlocks = new GameObject[5];

		float roofHeight = Random.Range (0.0f, 0.5f * houseScale.y);
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
			roofBlocks [2] = TriangleMesh (vertPos, true, null, texScale);

			for (int i = 0; i < vertPos.Length; i++) {
				vertPos [i].x += houseScale.x;
			}
			roofBlocks [3] = TriangleMesh (vertPos, false, null, texScale);
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
		Vector2 chimneyTexScale = new Vector2 (1.0f, chimneyScale.y);
		Vector3 chimneyPos = new Vector3
			(position.x + 0.01f + Random.Range (0.5f*chimneyScale.x, houseScale.x-0.5f*chimneyScale.x),
				position.y + houseScale.y + 0.5f*chimneyScale.y,
				position.z + 0.01f + Random.Range(0.5f*chimneyScale.z, houseScale.z-0.5f*chimneyScale.z));
		roofBlocks [4] = SetupPrimitive (PrimitiveType.Cube, new Vector3 (0.0f, 0.0f, 0.0f), chimneyScale, chimneyPos, roofTex, chimneyTexScale, "Building");

		return roofBlocks;
	}

	private GameObject TriangleMesh(Vector3[] position, bool reversed, Texture roofTex, Vector2 texScale)
	{
		GameObject triangle = new GameObject ();
		Mesh mesh = new Mesh ();
		triangle.AddComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = position;
		if (reversed) {
			mesh.triangles = new int[] { 2, 1, 0 };
		} else {
			mesh.triangles = new int[] { 0, 1, 2 };
		}
		mesh.uv = new Vector2[] { new Vector2 (0, 0), new Vector2 (0.5f*texScale.x, texScale.y), new Vector2 (texScale.x, 0) };
		triangle.AddComponent<MeshRenderer> ().material.mainTexture = roofTex;
		return triangle;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
