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

	public Vector3 minYardScale = new Vector3 (5.0f, 5.0f, 5.0f);
	public Vector3 maxYardScale = new Vector3 (10.0f, 10.0f, 10.0f);

	public Vector3 minLotScale = new Vector3 (5.0f, 5.0f, 5.0f);
	public Vector3 maxLotScale = new Vector3 (25.0f, 25.0f, 25.0f);

	public Vector3 minHouseScale = new Vector3 (5.0f, 5.0f, 5.0f);
	public Vector3 maxHouseScale = new Vector3 (10.0f, 10.0f, 10.0f);

	public Vector3 minBlockScale = new Vector3 (5.0f, 5.0f, 5.0f);
	public Vector3 maxBlockScale = new Vector3 (25.0f, 25.0f, 25.0f);

	// TODO:

	// Features:
	// more shapes
	// balconies
	// stairs?
	// path
	// more window setups
	// texture buildings and garages

	// Fixes:
	// door-window-roof clipping
	// further roof pimping (ridge, middle window)

	// Methods:
	// 2D garage / door / plot placement and pimping
	// rotation
	// symmetric displacement
	// texture-bool
	// re-organise and -scale
	// reuse designs
	// second pass?

	// Dissertation:
	// performance evaluation (size/area, quantity (20x20), primitive vs mesh, hardware)
	// write stuff

	// Use this for initialization
	void Start () {
		Random.seed = (int)System.DateTime.Now.Ticks;

		Roads roadNetwork = gameObject.AddComponent<Roads> ();
		Buildings houses = gameObject.AddComponent<Buildings> ();

		Vector3 plotPos = new Vector3(0.0f,0.0f,0.0f);
		Vector3 plotScale;
		Vector3 buildingPos;
		Vector3 buildingScale;

		for (int i = 0; i < numColumns; i++) {
			if (i == 0 || i == numColumns - 1) {
				plotScale.x = Random.Range (minYardScale.x, maxYardScale.x);
			} else {
				plotScale.x = Random.Range (minLotScale.x, maxLotScale.x);
			}
			for (int j = 0; j < numRows; j++) {
				//CHALLENGE: currentWidth = Random.Range (minBlockSpace, maxBlockSpace);
				if (j == 0 || j == numRows - 1) {
					plotScale.z = Random.Range (minYardScale.z, maxYardScale.z);
				} else {
					plotScale.z = Random.Range (minLotScale.z, maxLotScale.z);
				}

				plotScale.y = (int)Random.Range (minYardScale.y, maxYardScale.y);

				// Buildings
				if (i == 0 || i == numColumns - 1 || j == 0 || j == numRows - 1) {
					houses.SuburbanHouse (plotScale, plotPos, roofTex, windowTex);
				}

				else {
					houses.TownHouse (plotScale, plotPos, roofTex, windowTex);
				}


				// Plot
				plotScale = new Vector3 (plotScale.x, 0.0f, plotScale.z);
				Vector3 grassPos = plotPos + 0.5f*plotScale;
				Vector3 grassScale = new Vector3 (plotScale.x, plotScale.z, 0.1f);
				SetupPrimitive(PrimitiveType.Quad, new Vector3(90.0f,0.0f,0.0f), grassScale, grassPos, grassTex, 0.1f*grassScale, "Road");
				roadNetwork.RoadSection (plotScale, plotPos, streetSpace, roadTex, interTex);

				plotPos.z += plotScale.z+streetSpace;
			}
			plotPos.x += plotScale.x+streetSpace;
			plotPos.z = 0.0f;
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

	// Update is called once per frame
	void Update () {

	}
}
