using UnityEngine;
using System.Collections;

public class Build : MonoBehaviour {

	//http://texturelib.com/texture/?path=/Textures/road/road/road_road_0019
	//http://texturelib.com/texture/?path=/Textures/roof/roof_0075
	//http://texturelib.com/texture/?path=/Textures/grass/grass/grass_grass_0131
	//http://texturelib.com/texture/?path=/Textures/doors/wood%20doors/doors_wood_doors_0201
	public Texture roadTex;
	public Texture interTex;
	public Texture roofTex;
	public Texture windowTex;
	public Texture grassTex;
	public Texture doorTex;

	public int numRows = 15;
	public int numColumns = 10;

	public float streetSpace = 2.5f;

	public Vector3 minPlotScale = new Vector3 (5.0f, 5.0f, 5.0f);
	public Vector3 maxPlotScale = new Vector3 (20.0f, 25.0f, 20.0f);

	public Vector3 minBuildingScale = new Vector3 (5.0f, 5.0f, 5.0f);
	public Vector3 maxBuildingScale = new Vector3 (10.0f, 25.0f, 10.0f);

	// TODO:
	// more shapes
	// door-window-roof clipping
	// further roof pimping (ridge)
	// vertical stacking++
	// 2D garage / window (sort) / plot placement and pimping
	// stairs?
	// path
	// rotation
	// symmetric displacement
	// texture-bool
	// re-organise and -scale
	// reuse designs
	// performance evaluation (size/area, quantity (20x20), primitive vs mesh, hardware)
	// write stuff
	// second pass?

	// Use this for initialization
	void Start () {
		Random.seed = (int)System.DateTime.Now.Ticks;

		Vector3 currentPos = new Vector3(0.0f,0.0f,0.0f);
		Vector3 currentScale;
		Vector3 buildingPos;
		Vector3 buildingScale;

		for (int i = 0; i < numColumns; i++) {
			currentScale.x = Random.Range (minPlotScale.x, maxPlotScale.x);
			for (int j = 0; j < numRows; j++) {
				//CHALLENGE: currentWidth = Random.Range (minBlockSpace, maxBlockSpace);

				bool hasEntrance = false;

				currentScale.y = Random.Range (minPlotScale.y, maxPlotScale.y);
				currentScale.z = Random.Range (minPlotScale.z, maxPlotScale.z);
				buildingScale.x = Random.Range(minBuildingScale.x, currentScale.x);
				buildingScale.y = currentScale.y;
				buildingScale.z = Random.Range(minBuildingScale.z, currentScale.z);

				buildingPos = currentPos;
				buildingPos.x += Random.Range(0, currentScale.x - buildingScale.x);
				buildingPos.z += Random.Range(0, currentScale.z - buildingScale.z);

				if (i == 0 || i == numColumns - 1 || j == 0 || j == numRows - 1) {
					if (buildingScale.y > 7.0f) buildingScale.y = 7.0f;
					new Building (buildingScale, buildingPos, roofTex, windowTex);

					// doors, need path (single door with uv?)
					if (buildingPos.z - currentPos.z > currentPos.z + currentScale.z - (buildingPos.z + buildingScale.z)) {
						Vector3 doorScale = new Vector3 (2.0f, 3.0f, 0.1f);
						Vector3 doorPos = new Vector3 (buildingPos.x + (int)Random.Range (0.5f*doorScale.x, buildingScale.x-0.5f*doorScale.x), buildingPos.y + 0.5f*doorScale.y, buildingPos.z - 0.02f);
						SetupPrimitive (PrimitiveType.Quad, new Vector3 (0.0f, 0.0f, 0.0f), doorScale, doorPos, doorTex, new Vector2 (1.0f, 1.0f), "Building");
					} else { // why is it different?
						Vector3 doorScale = new Vector3 (2.0f, 3.0f, 0.1f);
						Vector3 doorPos = new Vector3 (buildingPos.x + (int)Random.Range (0.5f*doorScale.x, buildingScale.x-0.5f*doorScale.x), buildingPos.y + 0.5f*doorScale.y, buildingPos.z + buildingScale.z + 0.02f);
						SetupPrimitive (PrimitiveType.Quad, new Vector3 (0.0f, 180.0f, 0.0f), doorScale, doorPos, doorTex, new Vector2 (1.0f, 1.0f), "Building");
					}

					// garages
					if (buildingPos.x - currentPos.x > minBuildingScale.x) {
						Vector3 garageScale = new Vector3 (minBuildingScale.x, 3.0f, 0.5f * buildingScale.z);
						Vector3 garagePosition = buildingPos;
						garagePosition.x -= garageScale.x;
						new Building (garageScale, garagePosition, null, null);
					}
					else if ((currentPos.x + currentScale.x) - (buildingPos.x + buildingScale.x) > minBuildingScale.x) {
						Vector3 garageScale = new Vector3 (minBuildingScale.x, 3.0f, 0.5f * buildingScale.z);
						Vector3 garagePosition = buildingPos;
						garagePosition.x += buildingScale.x;
						new Building (garageScale, garagePosition, null, null);
					}
				}

				else {
					Vector3 thisScale = new Vector3 (Random.Range(minBuildingScale.x, buildingScale.x), (int)Random.Range (minBuildingScale.y, buildingScale.y)+0.5f, Random.Range(minBuildingScale.z, buildingScale.z));
					Vector3 thisPos = buildingPos;
					while (thisPos.x < currentPos.x+currentScale.x - thisScale.x) {
						while (thisPos.z < currentPos.z+currentScale.z - thisScale.z) {
							Texture hasRoof;
							if (Random.Range (0, 10) == 0)
								hasRoof = roofTex;
							else
								hasRoof = null;
							new Building (thisScale, thisPos, hasRoof, windowTex);

							//vertical stacking
							Vector3 oldScale = thisScale;
							Vector3 oldPos = thisPos;
							while (oldScale.y < maxBuildingScale.y-minBuildingScale.y && oldScale.x > minBuildingScale.x+1.0f && oldScale.z > minBuildingScale.z+1.0f) {
								Vector3 newScale = new Vector3 (Random.Range (minBuildingScale.x, oldScale.x), (int)Random.Range (minBuildingScale.y, maxBuildingScale.y - oldScale.y), Random.Range (minBuildingScale.z, oldScale.z));
								Vector3 newPos = new Vector3 (Random.Range (oldPos.x, oldPos.x + oldScale.x - newScale.x), oldPos.y + oldScale.y, Random.Range (oldPos.z, oldPos.z + oldScale.z - newScale.z));
								new Building (newScale, newPos, hasRoof, windowTex);
								oldScale = newScale;
								oldPos = newPos;
							}

							thisPos.z += thisScale.z;
							thisScale.y = Random.Range ((int)minBuildingScale.y, (int)maxBuildingScale.y)+0.5f;
							thisScale.z = Random.Range (minBuildingScale.z, maxBuildingScale.z);
						}
						thisPos.x += thisScale.x;
						thisPos.z = buildingPos.z;
						thisScale.x = Random.Range (minBuildingScale.x, maxBuildingScale.x);
					}
				}


				// Not local?
				Vector3 plotScale = new Vector3 (currentScale.x, 0.0f, currentScale.z);
				Vector3 grassPos = currentPos + 0.5f*plotScale;
				Vector3 grassScale = new Vector3 (currentScale.x, currentScale.z, 0.1f);
				SetupPrimitive(PrimitiveType.Quad, new Vector3(90.0f,0.0f,0.0f), grassScale, grassPos, grassTex, 0.1f*currentScale, "Road");
				new RoadSection (plotScale, currentPos, streetSpace, roadTex, interTex);

				currentPos.z += currentScale.z+streetSpace;
			}
			currentPos.x += currentScale.x+streetSpace;
			currentPos.z = 0.0f;
		}
	}


	public static GameObject SetupPrimitive(PrimitiveType type, Vector3 rotation, Vector3 scale, Vector3 position, Texture texture, Vector2 texScale, string tag)
	{
		GameObject shape = GameObject.CreatePrimitive (type);
		shape.transform.Rotate (rotation);
		shape.transform.localScale = scale;
		shape.transform.position = position;

		if (texture != null) {
			texture.wrapMode = TextureWrapMode.Repeat;
			MeshRenderer renderer = shape.GetComponent<MeshRenderer> ();
			renderer.material.mainTexture = texture;

			Mesh mesh = shape.GetComponent<MeshFilter> ().mesh;
			Vector2[] newUVs = mesh.uv;
			for (int i = 0; i < newUVs.Length; i++) {
				newUVs [i].x *= texScale.x;
				newUVs [i].y *= texScale.y;
			}
			mesh.uv = newUVs;
		}

		shape.tag = tag;

		return shape;
	}


	public class RoadSection
	{
		private GameObject[] segments = new GameObject[4];
		public RoadSection(Vector3 scale, Vector3 position, float streetSpace, Texture roadTex, Texture interTex)
		{
			Vector3 flatRotation = new Vector3(90.0f, 0.0f, 0.0f);
			Vector3 bottomLeftCorner = position - 0.5f*new Vector3(streetSpace, 0, streetSpace);
			Vector3 bottomRightCorner = new Vector3 (bottomLeftCorner.x+scale.x+streetSpace, 0.01f, bottomLeftCorner.z);
			Vector3 segmentScale = new Vector3(streetSpace, streetSpace, 0.1f);
			Vector2 texScale = new Vector2(1.0f, 1.0f);
			segments[0] = SetupPrimitive(PrimitiveType.Quad, flatRotation, segmentScale, bottomLeftCorner, interTex, texScale, "Road");
			segments[1] = SetupPrimitive(PrimitiveType.Quad, flatRotation, segmentScale, bottomRightCorner, interTex, texScale, "Road");

			Vector3 longScale = new Vector3(streetSpace, scale.z, 0.1f);
			Vector3 longPosition = new Vector3(position.x-0.5f*streetSpace, position.y, position.z+0.5f*scale.z);
			texScale = new Vector2(1.0f, longScale.y);
			segments[2] = SetupPrimitive(PrimitiveType.Quad, flatRotation, longScale, longPosition, roadTex, texScale, "Road");

			Vector3 wideRotation = new Vector3(90.0f, 0.0f, 90.0f);
			Vector3 wideScale = new Vector3(streetSpace, scale.x, 0.1f);
			Vector3 widePosition = new Vector3(position.x+0.5f*scale.x, position.y, position.z-0.5f*streetSpace);
			texScale = new Vector2(1.0f, wideScale.y);
			segments[3] = SetupPrimitive(PrimitiveType.Quad, wideRotation, wideScale, widePosition, roadTex, texScale, "Road");
		}
	}


	public class Building
	{
		private GameObject block;
		private GameObject[] roofBlocks;
		//private GameObject[] windowBlocks;

		public Building(Vector3 scale, Vector3 position, Texture roofTex, Texture windowTex)
		{
			block = new GameObject();

			Vector3 houseScale = scale;
			Vector3 housePosition = position + 0.5f*houseScale;
			Vector3 houseRotation = new Vector3(0.0f, 0.0f, 0.0f);
			Vector2 texScale = new Vector2(1.0f, 1.0f);
			block = SetupPrimitive(PrimitiveType.Cube, houseRotation, houseScale, housePosition, null, texScale, "Building");

			if (roofTex != null)
			{
				//roofBlocks = new GameObject[5];
				roofBlocks = RightAngleRoof(0, houseScale, position, roofTex);
			}

			if (windowTex != null)
			{
				Windows(position, scale, windowTex);
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
			//REDUNDANTwindowPosition.y += windowHeight + 0.5f*windowScale.y;
			windowPosition.z -= 0.01f;
			Vector2 texScale = new Vector2(1.0f, 1.0f);

			while (windowPosition.x < position.x+scale.x-windowScale.y)
			{
				windowPosition.y = position.y+windowHeight;
				while (windowPosition.y < position.y+scale.y-windowScale.y)
				{
					//windowBlocks [windowNum] = new GameObject ();
					if (Random.Range(0, 2) == 0) SetupPrimitive(PrimitiveType.Quad, new Vector3(0.0f, 0.0f, 0.0f), windowScale, windowPosition, windowTex, texScale, "Building");
					windowPosition.y += floorHeight;
				}
				windowPosition.x += floorHeight; // Dedicated value?
			}

			windowPosition = position;
			windowPosition.x += scale.x + 0.01f;
			windowPosition.y += windowHeight;
			windowPosition.z += windowHeight;
			Vector3 rotation = new Vector3 (0.0f, -90.0f, 0.0f);

			while (windowPosition.z < position.z+scale.z-windowScale.y)
			{
				windowPosition.y = position.y+windowHeight;
				while (windowPosition.y < position.y+scale.y-windowScale.y)
				{
					//windowBlocks [windowNum] = new GameObject ();
					if (Random.Range(0, 2) == 0) SetupPrimitive(PrimitiveType.Quad, rotation, windowScale, windowPosition, windowTex, texScale, "Building");
					windowPosition.y += floorHeight;
				}
				windowPosition.z += floorHeight; // Dedicated value?
			}

			windowPosition = position;
			windowPosition.x += windowHeight;
			windowPosition.y += windowHeight;
			windowPosition.z += scale.z + 0.01f;
			rotation = new Vector3 (0.0f, 180.0f, 0.0f);

			while (windowPosition.x < position.x+scale.x-windowScale.y)
			{
				windowPosition.y = position.y+windowHeight;
				while (windowPosition.y < position.y+scale.y-windowScale.y)
				{
					//windowBlocks [windowNum] = new GameObject ();
					if (Random.Range(0, 2) == 0) SetupPrimitive(PrimitiveType.Quad, rotation, windowScale, windowPosition, windowTex, texScale, "Building");
					windowPosition.y += floorHeight;
				}
				windowPosition.x += floorHeight; // Dedicated value?
			}

			windowPosition = position;
			windowPosition.x -= 0.01f;
			windowPosition.y += windowHeight;
			windowPosition.z += windowHeight;
			rotation = new Vector3 (0.0f, 90.0f, 0.0f);

			while (windowPosition.z < position.z+scale.z-windowScale.y)
			{
				windowPosition.y = position.y+windowHeight;
				while (windowPosition.y < position.y+scale.y-windowScale.y)
				{
					//windowBlocks [windowNum] = new GameObject ();
					if (Random.Range(0, 2) == 0) SetupPrimitive(PrimitiveType.Quad, rotation, windowScale, windowPosition, windowTex, texScale, "Building");
					windowPosition.y += floorHeight;
				}
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
	}

	// Update is called once per frame
	void Update () {

	}
}
