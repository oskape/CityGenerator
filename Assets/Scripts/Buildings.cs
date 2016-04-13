using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buildings : Build {

	private Vector3 minBuildingScale;
	private Vector3 maxBuildingScale;

	private float floorHeight;

	private Texture plotTexture;
	private Texture[] buildingTextures;
	private Texture roofTexture;

	public int windowCount;

	bool doorAdded;
	feature door;

	private bool stacking;

	feature[] availableFeatures;
	List<feature> remainingFeatures;

	public void InitBuildings(bool aStacking, Texture aPlotTexture, Vector3 aMinBuildingScale, Vector3 aMaxBuildingScale, float aFloorHeight, Texture[] aBuildingTextures, Texture aRoofTexture)
	{
		stacking = aStacking;

		plotTexture = aPlotTexture;

		minBuildingScale = aMinBuildingScale;
		maxBuildingScale = aMaxBuildingScale;

		floorHeight = aFloorHeight;

		buildingTextures = new Texture[aBuildingTextures.Length];
		for (int i = 0; i < aBuildingTextures.Length; i++) {
			buildingTextures[i] = aBuildingTextures[i];
		}
		
		roofTexture = aRoofTexture;
	}

	public void SetFeatures(feature[] aFeatures)
	{
		availableFeatures = aFeatures;
	}

	public GameObject SetupPlot(Vector3 plotScale)
	{
		Vector3 buildingScale = new Vector3 ((int)Random.Range (minBuildingScale.x, maxBuildingScale.x), (int)Random.Range (minBuildingScale.y, maxBuildingScale.y), (int)Random.Range (minBuildingScale.z, maxBuildingScale.z));
		buildingScale.x = (int)Mathf.Clamp (buildingScale.x, 0, plotScale.x);
		buildingScale.z = (int)Mathf.Clamp (buildingScale.z, 0, plotScale.z);

		Vector3 buildingOffset = new Vector3 (Random.Range (0.0f, plotScale.x - buildingScale.x), 0.0f, Random.Range (0.0f, plotScale.z - buildingScale.z));

		if (stacking)
			return BlockPlot (plotScale, buildingScale, buildingOffset);
		else
			return HousePlot (plotScale, buildingScale, buildingOffset);
		
	}

	private GameObject HousePlot(Vector3 plotScale, Vector3 buildingScale, Vector3 buildingOffset)
	{
		GameObject plot = QuadMesh (plotScale, plotTexture, new Vector2 (0.05f * plotScale.x, 0.05f * plotScale.z), "Road");

		if (buildingScale.x < minBuildingScale.x || buildingScale.z < minBuildingScale.z) {
			return plot;
		}

		GameObject building = BoxBuilding (buildingScale, buildingOffset, true);
		building.transform.SetParent (plot.transform, false);

		AddFeatures (building, buildingScale, new Vector4 (buildingOffset.z, plotScale.z - (buildingOffset.z + buildingScale.z), buildingOffset.x, plotScale.x - (buildingOffset.x + buildingScale.x)));

		return plot;
	}

	public GameObject BlockPlot(Vector3 plotScale, Vector3 buildingScale, Vector3 initOffset)
	{
		GameObject plot = QuadMesh (plotScale, plotTexture, new Vector2 (0.05f * plotScale.x, 0.05f * plotScale.z), "Road");

		if (buildingScale.x < minBuildingScale.x || buildingScale.z < minBuildingScale.z) {
			return plot;
		}

		Vector3 buildingOffset = initOffset;

		while (buildingOffset.x + buildingScale.x <= plotScale.x) {
			while (buildingOffset.z + buildingScale.z <= plotScale.z) {
				Vector3 thisScale = buildingScale;
				Vector3 thisOffset = buildingOffset;
				bool goneCircular = false;

				while (thisOffset.y + thisScale.y <= maxBuildingScale.y) {

					if (goneCircular || (thisScale.x == thisScale.z && thisScale.x >= 10.0f)) {
						goneCircular = true;

						Texture thisTexture = new Texture ();
						if (buildingTextures.Length >= 2)
							thisTexture = buildingTextures [1];

						GameObject building = ConeMesh (true, thisScale, thisTexture, 1.0f, "Building");
						building.transform.Translate (0.5f * thisScale.x, 0, 0.5f * thisScale.z);
						building.transform.Translate (thisOffset);
						building.transform.SetParent (plot.transform, false);

						Vector3 roofScale = new Vector3 (thisScale.x, 0.2f * thisScale.y, 0.0f);
						GameObject roof = ConeMesh (false, roofScale, roofTexture, 1.0f, "Building");
						roof.transform.SetParent (building.transform, false);
						roof.transform.Translate (0.0f, thisScale.y, 0.0f);

						break;
					} else {
						GameObject building = BoxBuilding (thisScale, thisOffset, true);
						building.transform.SetParent (plot.transform, false);

						AddFeatures (building, thisScale, new Vector4(0.1f, 0.1f, 0.1f, 0.1f));
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
				buildingScale.z = (int)Random.Range (minBuildingScale.z, maxBuildingScale.z);
				buildingScale.y = (int)Random.Range (minBuildingScale.y, maxBuildingScale.y);
			}
			buildingOffset.z = initOffset.z;
			buildingOffset.x += buildingScale.x;
			buildingScale.x = (int)Random.Range (minBuildingScale.x, maxBuildingScale.x);
		}

		return plot;
	}

	public GameObject BoxBuilding(Vector3 scale, Vector3 offset, bool hasChimney)
	{
		Texture thisTexture = new Texture();
		if (buildingTextures.Length != 0) {
			thisTexture = buildingTextures [0];
		}

		GameObject thisBuilding = BoxMesh (scale, thisTexture, 0.3f, "Building");
		thisBuilding.transform.Translate (offset);

		GameObject[] roof = RightAngleRoof (scale, hasChimney);
		for (int i = 0; i < roof.Length; i++) {
			roof[i].transform.SetParent (thisBuilding.transform, false);
		}

		return thisBuilding;
	}

	public GameObject Garage(Vector3 scale, Texture gateTexture)
	{
		GameObject garage = BoxBuilding (scale, new Vector3 (0.0f, 0.0f, 0.0f), false);
		GameObject gate = QuadMesh (new Vector3 (scale.z, scale.y-1.0f, 0.0f), gateTexture, new Vector2 (1.0f, 1.0f), "Building");

		Vector3 gateOffset = new Vector3(-scale.z, 0.0f, -0.01f);
		Vector3 gateRotation = new Vector3 (0.0f, 90.0f, 0.0f);
		if (Random.Range (0, 2) == 0) {
			gateOffset = new Vector3 (0.0f, 0.0f, -scale.x - 0.01f);
			gateRotation.y = -gateRotation.y;
		}
		
		gate.transform.Rotate (gateRotation);
		gate.transform.Translate (gateOffset);
		gate.transform.SetParent (garage.transform, false);

		return garage;
	}

	private void AddFeatures(GameObject building, Vector3 thisScale, Vector4 plotSpaces)
	{
		doorAdded = false;

		remainingFeatures = new List<feature> ();
		for (int i = 0; i < availableFeatures.Length; i++) {
			if (availableFeatures [i].name != "Door") {
				remainingFeatures.Add (availableFeatures [i]);
			} else {
				door = availableFeatures [i];
			}
		}

		FeatureWall (building, new Vector3 (thisScale.x, thisScale.y, plotSpaces.x), 0.0f, new Vector3 (0.0f, 0.0f, -0.01f), new Vector2(0.0f, 2.0f*thisScale.x+2.0f*thisScale.z));

		FeatureWall (building, new Vector3 (thisScale.x, thisScale.y, plotSpaces.y), 180.0f, new Vector3 (-thisScale.x, 0.0f, -thisScale.z - 0.01f), new Vector2(thisScale.x, 2.0f*thisScale.x+2.0f*thisScale.z));

		FeatureWall (building, new Vector3 (thisScale.z, thisScale.y, plotSpaces.z), 90.0f, new Vector3 (-thisScale.z, 0.0f, -0.01f), new Vector2 (2.0f * thisScale.x, 2.0f * thisScale.x + 2.0f * thisScale.z));

		FeatureWall (building, new Vector3 (thisScale.z, thisScale.y, plotSpaces.w), -90.0f, new Vector3 (0.0f, 0.0f, -thisScale.x - 0.01f), new Vector2(2.0f*thisScale.x + thisScale.z, 2.0f*thisScale.x+2.0f*thisScale.z));
	}

	private void FeatureWall(GameObject building, Vector3 wallScale, float rotation, Vector3 translation, Vector2 doorOdds)
    {
		Vector3 position = new Vector3 (0.0f, 0.0f, 0.0f);

        Vector3 initialPosition = position;

		while (position.y < wallScale.y)
        {
            while (position.x < wallScale.x)
            {
				// Select new feature
				feature thisFeature;

				if ((Random.Range (0.0f, doorOdds.y - 2.0f*door.spacing.x - door.scale.x) <= doorOdds.x + position.x) && !doorAdded && position.y + building.transform.position.y == 0.0f) {
					thisFeature = door;
				} else {
					do {
						thisFeature = remainingFeatures.ToArray () [Random.Range (0, remainingFeatures.ToArray ().Length)];
					} while (thisFeature.grounded && position.y + building.transform.position.y > 0.0f);
				}

				if (position.x + 2.0f*thisFeature.spacing.x + thisFeature.scale.x <= wallScale.x && 
					position.y + 2.0f*thisFeature.spacing.y + thisFeature.scale.y <= wallScale.y && 
					position.z + thisFeature.scale.z + thisFeature.spacing.z <= wallScale.z) {

					position.x += thisFeature.spacing.x;
					Vector3 thisPosition = position;

					thisPosition.y += thisFeature.spacing.y;
					thisPosition.z -= thisFeature.scale.z;
					thisPosition.z -= thisFeature.spacing.z;

					if (Random.Range (0.0f, thisFeature.odds.y) <= thisFeature.odds.x) {
						GameObject thisShape = (GameObject)Instantiate (thisFeature.shape, thisPosition, Quaternion.identity);
						thisShape.transform.RotateAround (initialPosition, new Vector3 (0.0f, 1.0f, 0.0f), rotation);
						thisShape.transform.Translate (translation);
						thisShape.SetActive (true);
						thisShape.transform.SetParent (building.transform, false);

						if (thisFeature.name == "Window") {
							windowCount++;
						} else if (thisFeature.name == "Door") {
							doorAdded = true;
						} else if (thisFeature.single) {
							remainingFeatures.Remove (thisFeature);
						}
					}
					position.x += thisFeature.scale.x;
				}
				position.x += thisFeature.spacing.x;
            }
            position.x = initialPosition.x;
			position.y += floorHeight;
        }
    }
		
	private GameObject[] RightAngleRoof(Vector3 houseScale, bool hasChimney)
	{
		List<GameObject> roofBlocks = new List<GameObject> ();

		float roofHeight = (int)Random.Range (0.0f, 0.5f * houseScale.y);
		float angle = Mathf.Atan2 (roofHeight, 0.5f*houseScale.z);

		Vector3 roofScale = new Vector3 (houseScale.x, 0.0f, 0.5f * houseScale.z / Mathf.Cos (angle));

		if (Random.Range (0, 2) == 0) {

			Vector3 slopeRotation = new Vector3(-(angle * Mathf.Rad2Deg), 0.0f, 0.0f);
			GameObject roofBlock = QuadMesh (roofScale, roofTexture, new Vector2(roofScale.x, roofScale.z), "Building");//SetupPrimitive (PrimitiveType.Quad, rotationDegrees, roofScale, roofPosition1, roofTexture, texScale, "Building");
			roofBlock.transform.Translate (0.0f, houseScale.y, 0.0f);
			roofBlock.transform.Rotate (slopeRotation);
			roofBlocks.Add (roofBlock);

			slopeRotation.y += 180.0f;
			roofBlock = QuadMesh (roofScale, roofTexture, new Vector2(roofScale.x, roofScale.z), "Building");//SetupPrimitive (PrimitiveType.Quad, rotationDegrees, roofScale, roofPosition2, roofTexture, texScale, "Building");
			roofBlock.transform.Translate (houseScale.x, houseScale.y, houseScale.z);
			roofBlock.transform.Rotate (slopeRotation);
			roofBlocks.Add (roofBlock);

			Vector3[] vertPos = new Vector3[] {
				new Vector3 (0.0f, 0.0f, 0.0f),
				new Vector3 (0.0f, roofHeight, 0.5f*houseScale.z),
				new Vector3 (0.0f, 0.0f, houseScale.z)};

			Texture thisTexture = new Texture ();
			if (buildingTextures.Length != 0)
				thisTexture = buildingTextures [0];
			
			roofBlock = TriangleMesh (vertPos, true, thisTexture, new Vector2 (0.3f * houseScale.z, 0.3f * roofHeight), "Building");
			roofBlock.transform.Translate (0.0f, houseScale.y, 0.0f);
			roofBlocks.Add (roofBlock);

			roofBlock = TriangleMesh (vertPos, false, thisTexture, new Vector2 (0.3f * houseScale.z, 0.3f * roofHeight), "Building");
			roofBlock.transform.Translate (houseScale.x, houseScale.y, 0.0f);
			roofBlocks.Add (roofBlock);
		}

		else {
			Vector3 cornerSW = new Vector3 (0.0f, 0.0f, 0.0f);
			Vector3 cornerSE = new Vector3 (houseScale.x, 0.0f, 0.0f);
			Vector3 cornerNW = new Vector3 (0.0f, 0.0f, houseScale.z);
			Vector3 cornerNE = new Vector3 (houseScale.x, 0.0f, houseScale.z);
			Vector3 top = new Vector3 (0.5f * houseScale.x, roofHeight, 0.5f * houseScale.z);

			Vector2 texScaleWide = new Vector2 (houseScale.x, houseScale.z);
			Vector2 texScaleLong = new Vector2 (houseScale.z, Mathf.Sqrt (Mathf.Pow (roofHeight, 2) + Mathf.Pow (houseScale.x * 0.5f, 2)));

			Vector3[] vertPos = new Vector3[] { cornerSW, top, cornerSE };
			GameObject roofBlock = TriangleMesh (vertPos, false, roofTexture, texScaleWide, "Building");
			roofBlock.transform.Translate (0.0f, houseScale.y, 0.0f);
			roofBlocks.Add (roofBlock);

			vertPos = new Vector3[] { cornerSE, top, cornerNE };
			roofBlock = TriangleMesh (vertPos, false, roofTexture, texScaleLong, "Building");
			roofBlock.transform.Translate (0.0f, houseScale.y, 0.0f);
			roofBlocks.Add (roofBlock);

			vertPos = new Vector3[] { cornerNE, top, cornerNW };
			roofBlock = TriangleMesh (vertPos, false, roofTexture, texScaleWide, "Building");
			roofBlock.transform.Translate (0.0f, houseScale.y, 0.0f);
			roofBlocks.Add (roofBlock);

			vertPos = new Vector3[] { cornerNW, top, cornerSW };
			roofBlock = TriangleMesh (vertPos, false, roofTexture, texScaleLong, "Building");
			roofBlock.transform.Translate (0.0f, houseScale.y, 0.0f);
			roofBlocks.Add (roofBlock);
		}

		if (hasChimney) {
			Vector3 chimneyScale = new Vector3 (1.0f, roofHeight, 1.0f);
			if (roofHeight < 1.0f)
				chimneyScale.y = 1.0f;
			Vector2 chimneyTexScale = new Vector2 (1.0f, chimneyScale.y);
			Vector3 chimneyPos = new Vector3 (0.01f + Random.Range (0.5f * chimneyScale.x, houseScale.x - 0.5f * chimneyScale.x),
				houseScale.y + 0.5f * chimneyScale.y,
				0.01f + Random.Range (0.5f * chimneyScale.z, houseScale.z - 0.5f * chimneyScale.z));
			GameObject roofBlock = SetupPrimitive (PrimitiveType.Cube, new Vector3 (0.0f, 0.0f, 0.0f), chimneyScale, chimneyPos, roofTexture, chimneyTexScale, "Building");
			roofBlocks.Add (roofBlock);
		}

		return roofBlocks.ToArray ();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
