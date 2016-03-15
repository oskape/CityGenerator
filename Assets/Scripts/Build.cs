using UnityEngine;
using System.Collections;

public class Build : MonoBehaviour {

	// TODO:

	// Features:
	// more shapes
	// balconies
	// stairs?
	// path
	// more window setups
	// texture buildings and garages
	// reset

	// Fixes:
	// door-window-roof clipping
	// further roof pimping (ridge, middle window)
	// limit roofs

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

	private Roads roadNetwork;
	private Buildings houses;

	private GameObject[] roads;
	private GameObject[] buildings;

	// Use this for initialization
	void Start () {
		Random.seed = (int)System.DateTime.Now.Ticks;

		roadNetwork = gameObject.AddComponent<Roads> ();
		houses = gameObject.AddComponent<Buildings> ();

		//roads = new GameObject[numRows * numColumns];

		Init ();
	}

	public void CleanWorld()
	{
		GameObject[] GameObjects = (FindObjectsOfType<GameObject> () as GameObject[]);

		for (int i = 0; i < GameObjects.Length; i++) {
			if (GameObjects [i].tag != "MainCamera" && GameObjects [i].tag != "Light") {
				Destroy (GameObjects [i]);
			}
		}
	}

	public void Init()
	{
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
					houses.SuburbanHouse (plotScale, plotPos, roofTex, windowTex, doorTex);
				}

				else {
					houses.TownHouse (plotScale, plotPos, roofTex, windowTex, doorTex);
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

	protected GameObject SetupPrimitive(PrimitiveType type, Vector3 rotation, Vector3 scale, Vector3 position, Texture texture, Vector2 texScale, string tag)
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

	protected GameObject BoxMesh(Vector3 scale, Vector3 position, Texture texture, Vector2 textureScale, string tag)
	{
		GameObject box = new GameObject ();
		Mesh mesh = new Mesh ();
		box.AddComponent<MeshFilter> ().mesh = mesh;

		Vector3[] vertices = new Vector3[8];

		// front
		vertices [0] = position; // bottom left
		vertices [1] = new Vector3 (position.x + scale.x, position.y, position.z); // bottom right
		vertices [2] = new Vector3 (position.x + scale.x, position.y + scale.y, position.z); // top right
		vertices [3] = new Vector3 (position.x, position.y + scale.y, position.z); // top left

		// back
		position.z += scale.z;
		vertices [4] = position; // bottom left
		vertices [5] = new Vector3 (position.x + scale.x, position.y, position.z); // bottom right
		vertices [6] = new Vector3 (position.x + scale.x, position.y + scale.y, position.z); // top right
		vertices [7] = new Vector3 (position.x, position.y + scale.y, position.z); // top left

		Vector3[] moreVertices = new Vector3[] {
			// front
			vertices [0], vertices [1], vertices [2], vertices [3],
			// back
			vertices [5], vertices [4], vertices [7], vertices [6],
			// left
			vertices [4], vertices [0], vertices [3], vertices [7],
			// right
			vertices [1], vertices [5], vertices [6], vertices [2],
			// top
			vertices [3], vertices [2], vertices [6], vertices [7]
		};

		Vector2 xBL = new Vector2 (0.0f, 0.0f);
		Vector2 xBR = new Vector2 (scale.x, 0.0f);
		Vector2 xTR = new Vector2 (scale.x, scale.y);
		Vector2 xTL = new Vector2 (0.0f, scale.y);

		Vector2 zBL = new Vector2 (0.0f, 0.0f);
		Vector2 zBR = new Vector2 (scale.z, 0.0f);
		Vector2 zTR = new Vector2 (scale.z, scale.y);
		Vector2 zTL = new Vector2 (0.0f, scale.y);

		Vector2[] UVs = new Vector2[] {
			xBL, xBR, xTR, xTL,
			xBL, xBR, xTR, xTL,
			zBL, zBR, zTR, zTL,
			zBL, zBR, zTR, zTL,
			new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector2(0.0f, 1.0f)
		};
			
		Vector3[] normals = new Vector3[] {
			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
			Vector3.up, Vector3.up, Vector3.up, Vector3.up
		};

		mesh.vertices = moreVertices;
		mesh.uv = UVs;
		// only half the lenght on top?
		mesh.normals = normals;

		mesh.triangles = new int[] {
			// front
			2, 1, 0,
			0, 3, 2,
//			2, 1, 0,
//			0, 3, 2,
			// back
			6, 5, 4,
			4, 7, 6,
//			7, 4, 5,
//			5, 6, 7,
			// left
			10, 9, 8,
			8, 11, 10,
//			3, 0, 4,
//			4, 7, 3,
			// right
			14, 13, 12,
			12, 15, 14,
//			6, 5, 1,
//			1, 2, 6,
			// top
			7, 2, 3,
			3, 6, 7
//			6, 2, 3,
//			3, 7, 6
		};

		//mesh.uv = new Vector2[] { new Vector2 (0, 0), new Vector2 (0.5f*textureScale.x, textureScale.y), new Vector2 (textureScale.x, 0) };
		box.AddComponent<MeshRenderer> ().material.mainTexture = texture;

		return box;
	}

	protected GameObject TriangleMesh(Vector3[] position, bool reversed, Texture roofTex, Vector2 texScale)
	{
		GameObject triangle = new GameObject ();
		Mesh mesh = new Mesh ();
		triangle.AddComponent<MeshFilter> ().mesh = mesh;

		mesh.vertices = position;
		//triangle.transform.RotateAround(// (0.0f, 90.0f, 0.0f);
		if (reversed) {
			mesh.triangles = new int[] { 2, 1, 0 };
		} else {
			mesh.triangles = new int[] { 0, 1, 2 };
		}
		mesh.uv = new Vector2[] { new Vector2 (0, 0), new Vector2 (0.5f*texScale.x, texScale.y), new Vector2 (texScale.x, 0) };
		triangle.AddComponent<MeshRenderer> ().material.mainTexture = roofTex;

		return triangle;
	}

	// Update is called once per frame
	void Update () {

	}
}
