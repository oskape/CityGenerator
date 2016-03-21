using UnityEngine;
using System.Collections;

public class Buildings : Build {

	private GameObject block;
	private GameObject[] roofBlocks;
	//private GameObject[] windowBlocks;

	public void MakeBuilding(Vector3 scale, Vector3 position, Texture roofTex, Texture windowTex, Texture buildingTex)
	{
		block = new GameObject();

		Vector3 houseScale = scale;
		Vector3 housePosition = position;// + 0.5f*houseScale;
		Vector3 houseRotation = new Vector3(0.0f, 0.0f, 0.0f);
		float texScale = 0.3f;

		//block = BoxMesh (houseScale, housePosition, buildingTex, texScale, "Building");//SetupPrimitive(PrimitiveType.Cube, houseRotation, houseScale, housePosition, null, texScale, "Building");


		if (roofTex != null)
		{
			//roofBlocks = new GameObject[5];
			//roofBlocks = RightAngleRoof(0, scale, position, roofTex);
		}

		if (windowTex != null)
		{
			Windows(position, scale, windowTex);
		}
	}

	public void SuburbanHouse(Vector3 plotScale, /*Vector3 plotPosition*/ GameObject plot, Texture roofTex, Texture windowTex, Texture doorTex, Texture buildingTex)
	{
		Vector3 buildingScale;
		buildingScale.x = Random.Range(minHouseScale.x, plotScale.x);
		buildingScale.y = plotScale.y;
		buildingScale.z = Random.Range(minHouseScale.z, plotScale.z);

		Vector3 offset = new Vector3 (Random.Range (0, plotScale.x - buildingScale.x), 0, Random.Range (0, plotScale.z - buildingScale.z));

		GameObject building = BoxMesh(buildingScale, buildingTex, 0.3f, "Building");
		building.transform.SetParent (plot.transform, false);
		building.transform.Translate (offset);
		GameObject[] roof = RightAngleRoof (0, buildingScale, new Vector3 (0, 0, 0), roofTex, buildingTex);
		for (int i = 0; i < roof.Length; i++) {
			roof[i].transform.SetParent (building.transform, false);
		}

		// doors, need path (single door with uv?)
		Vector3 doorScale = new Vector3 (2.0f, 0.0f, 3.0f);
		GameObject door = QuadMesh (doorScale, doorTex, 1.0f, "Building");
		door.transform.SetParent (building.transform, false);
		Vector3 doorRotation = new Vector3 (-90.0f, 0.0f, 0.0f);
		Vector3 doorOffset = new Vector3 ((int)Random.Range (0.0f, buildingScale.x-doorScale.x), 0.0f, -0.02f);
		if (offset.z < plotScale.z - (offset.z + buildingScale.z)) {
			doorOffset.x += doorScale.x;
			doorOffset.z += buildingScale.z + 0.04f;
			doorRotation.y += 180.0f;
		}
		door.transform.Translate (doorOffset);
		door.transform.Rotate (doorRotation);

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

		Windows (plot.transform.position + offset, buildingScale, windowTex);
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

	public void TownHouse(GameObject plot, Vector3 plotScale, Texture buildingTex, Texture roofTex, Texture windowTex, Texture doorTex)
	{
		Vector3 offset = new Vector3(Random.Range(0, plotScale.x - minBlockScale.x), 0, Random.Range(0, plotScale.z - minBlockScale.z));
		Vector3 scale;// = new Vector3 (Random.Range (minBlockScale.x, plotScale.x - offset.x), Random.Range (minBlockScale.y, maxBlockScale.y - offset.y), Random.Range (minBlockScale.z, plotScale.z - offset.z));
		while (offset.x + minBlockScale.x <= plotScale.x) {
			scale.x = Random.Range (minBlockScale.x, plotScale.x - offset.x);
			while (offset.z + minBlockScale.z <= plotScale.z) {
				scale.z = Random.Range (minBlockScale.z, plotScale.z - offset.z);
				scale.y = (int)Random.Range (minBlockScale.y, maxBlockScale.y);
				Vector3 thisScale = scale;
				Vector3 thisOffset = offset;
				while (thisOffset.y + minBlockScale.y <= maxBlockScale.y) {
					GameObject building = BoxMesh (thisScale, buildingTex, 0.3f, "Building");
					building.transform.SetParent (plot.transform, false);
					building.transform.Translate (thisOffset);

					// laggyyyyy
					if (windowTex != null)
					{
						Windows (plot.transform.position+thisOffset, thisScale, windowTex);
					}

					GameObject[] roof = RightAngleRoof (0, thisScale, new Vector3(0,0,0), roofTex, buildingTex);
					for (int i = 0; i < roof.Length; i++) {
						roof [i].transform.SetParent (building.transform, false);
						//roof [i].transform.Translate (0.1f, 0, 0.1f);
					}

					thisOffset.y += thisScale.y;

					Vector3 oldScale = thisScale;
					thisScale = new Vector3 (Random.Range (minBlockScale.x, thisScale.x), (int)Random.Range (minBlockScale.y, maxBlockScale.y), Random.Range (minBlockScale.z, thisScale.z));

					thisOffset.x += Random.Range (0, oldScale.x - thisScale.x);
					thisOffset.z += Random.Range (0, oldScale.z - thisScale.z);
				}
				offset.z += scale.z;
			}
			offset.x += scale.x;
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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
