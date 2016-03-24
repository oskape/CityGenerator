using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Build : MonoBehaviour {

	// TODO:

	// Features:
	// more shapes
	// balconies
	// stairs?
	// path
	// more window setups
	// main roads

	// Fixes:
	// door-window-roof clipping
	// further roof pimping (ridge, middle window)

	// Methods:
	// 2D garage / door / plot placement and pimping
	// rotation
	// reuse designs
	// second pass?

	// Dissertation:
	// performance evaluation (size/area, quantity (20x20), primitive vs mesh, hardware)
	// write stuff

	//http://texturelib.com/texture/?path=/Textures/road/road/road_road_0019
	//http://texturelib.com/texture/?path=/Textures/road/bare%20asphalt/road_bare_asphalt_0035
	//http://texturelib.com/texture/?path=/Textures/roof/roof_0075
	//http://texturelib.com/texture/?path=/Textures/roof/roof_0101
	//http://texturelib.com/texture/?path=/Textures/grass/grass/grass_grass_0131
	//http://texturelib.com/texture/?path=/Textures/doors/wood%20doors/doors_wood_doors_0201
	//http://texturelib.com/texture/?path=/Textures/doors/metal%20doors/doors_metal_doors_0139
	//http://texturelib.com/texture/?path=/Textures/brick/modern/brick_modern_0123
	//http://texturelib.com/texture/?path=/Textures/wood/planks%20new/wood_planks_new_0042
	//http://texturelib.com/texture/?path=/Textures/windows/windows_0197
	//http://texturelib.com/texture/?path=/Textures/windows/windows_0192
	[SerializeField]private Texture roadTex;
	[SerializeField]private Texture interTex;

	[SerializeField]private Texture houseRoofTex;
	[SerializeField]private Texture blockRoofTex;

	[SerializeField]private Texture myWindowTex;
	[SerializeField]private Texture blockWindowTex;

	[SerializeField]private Texture grassTex;
	[SerializeField]private Texture paveTex;

	[SerializeField]private Texture houseDoorTex;
	[SerializeField]private Texture blockDoorTex;

	[SerializeField]private Texture[] houseTex;
	[SerializeField]private Texture[] blockTex;
	[SerializeField]private Texture glassTex;

	[SerializeField]private Vector2 mapScale = new Vector2 (400.0f, 400.0f);

	[SerializeField]private Vector2 minYardScale = new Vector2 (10.0f, 10.0f);
	[SerializeField]private Vector2 maxYardScale = new Vector2 (15.0f, 15.0f);

	[SerializeField]private Vector2 minLotScale = new Vector2 (10.0f, 10.0f);
	[SerializeField]private Vector2 maxLotScale = new Vector2 (30.0f, 30.0f);

	[SerializeField]private Vector3 minHouseScale = new Vector3 (7.0f, 5.0f, 7.0f);
	[SerializeField]private Vector3 maxHouseScale = new Vector3 (10.0f, 10.0f, 10.0f);

	[SerializeField]private Vector3 garageScale = new Vector3(7.0f, 5.0f, 7.0f);

	[SerializeField]private Vector3 minBlockScale = new Vector3 (7.0f, 5.0f, 7.0f);
	[SerializeField]private Vector3 maxBlockScale = new Vector3 (25.0f, 25.0f, 25.0f);

	[SerializeField]private int windowOdds = 2;
	[SerializeField]private Vector2 windowScale = new Vector2 (1.0f, 1.0f);
	[SerializeField]private Vector2 windowSpacing = new Vector2 (3.0f, 3.0f);
	[SerializeField]private Vector2 doorScale = new Vector2 (2.0f, 3.0f);

	[SerializeField]private float streetWidth = 5.0f;
	//[SerializeField]private float mainRoadWidth = 10.0f;

	private Buildings houses;
	private Buildings blocks;
	private Roads roads;

	// not used
	private int numRows = 15;
	private int numColumns = 10;

	// Use this for initialization
	void Start () {
		Random.seed = (int)System.DateTime.Now.Ticks;

		houses = gameObject.AddComponent<Buildings> ();
		blocks = gameObject.AddComponent<Buildings> ();
		roads = gameObject.AddComponent<Roads> ();

		houses.InitPlots (grassTex);
		houses.InitBuildings (false, minHouseScale, maxHouseScale, garageScale, houseTex, houseRoofTex);
		houses.InitWindows (windowOdds, windowScale, windowSpacing, myWindowTex);
		houses.InitDoors (doorScale, houseDoorTex);

		blocks.InitPlots (paveTex);
		blocks.InitBuildings (true, minBlockScale, maxBlockScale, garageScale, blockTex, blockRoofTex);
		blocks.InitWindows (windowOdds, windowScale, windowSpacing, blockWindowTex);
		blocks.InitDoors (doorScale, blockDoorTex);

		roads.InitRoads (streetWidth, roadTex, interTex);

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
		GeneratePlots ();
//		ColumnsShareWidth ();
	}

	private void GeneratePlots()
	{
		Vector3 currentPos = new Vector3(0.0f,0.0f,0.0f);

		while (currentPos.x < mapScale.x && currentPos.z < mapScale.y) {
			Vector2 plotScale = new Vector2 (Random.Range (minLotScale.x, maxLotScale.x), Random.Range (minLotScale.y, maxLotScale.y));
			Vector3 plotPos = currentPos;

			if (plotScale.x > plotScale.y) {
				
				// Generate row with shared length
				while (plotPos.x + plotScale.x < mapScale.x) {
					bool house = false;
					if (Random.Range (0.0f, mapScale.x - currentPos.x) < 0.5f * mapScale.x)
						house = true;
					CreatePlot (plotScale, plotPos, house);

					plotPos.x += plotScale.x + streetWidth;
					plotScale.x = Random.Range (minLotScale.x, maxLotScale.x);
				}

				currentPos.z += plotScale.y + streetWidth;
			} else {

				// Generate column with shared width
				while (plotPos.z + plotScale.y < mapScale.y) {
					bool house = false;
					if (Random.Range (0.0f, mapScale.y - currentPos.z) < 0.5f * mapScale.y)
						house = true;
					CreatePlot (plotScale, plotPos, house);

					plotPos.z += plotScale.y + streetWidth;
					plotScale.y = Random.Range (minLotScale.y, maxLotScale.y);
				}

				currentPos.x += plotScale.x + streetWidth;
			}
		}
	}

	private void CreatePlot(Vector2 plotScale, Vector3 plotPosition, bool house)
	{
		GameObject plot;
		if (house)
			plot = houses.SetupPlot (plotScale);
		else
			plot = blocks.SetupPlot (plotScale);
		plot.transform.Translate (plotPosition);

		GameObject[] roadSection = roads.RoadSection (plotScale);
		for (int a = 0; a < roadSection.Length; a++) {
			roadSection [a].transform.SetParent (plot.transform, false);
		}
	}

	private void ColumnsShareWidth()
	{
		Vector3 plotPos = new Vector3(0.0f,0.0f,0.0f);
		Vector2 plotScale = new Vector3(0.0f, 0.0f);

		for (int i = 0; i < numColumns; i++) {
			if (i == 0 || i == numColumns - 1) {
				plotScale.x = Random.Range (minYardScale.x, maxYardScale.x);
			} else {
				plotScale.x = Random.Range (minLotScale.x, maxLotScale.x);
			}
			for (int j = 0; j < numRows; j++) {
				if (j == 0 || j == numRows - 1) {
					plotScale.y = Random.Range (minYardScale.y, maxYardScale.y);
				} else {
					plotScale.y = Random.Range (minLotScale.y, maxLotScale.y);
				}

				// Plots
				if (i == 0 || i == numColumns - 1 || j == 0 || j == numRows - 1) {
					GameObject plot = houses.SetupPlot (plotScale);
					plot.transform.Translate (plotPos);
				}

				else {
					GameObject plot = blocks.SetupPlot (plotScale);
					plot.transform.Translate (plotPos);
				}

				plotPos.z += plotScale.y + streetWidth;
			}
			plotPos.x += plotScale.x+streetWidth;
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

	protected GameObject QuadMesh(Vector2 scale, Texture texture, Vector2 textureScale, string tag)
	{
		GameObject quad = new GameObject ();
		MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh ();
		meshFilter.mesh = mesh;

		mesh.vertices = new Vector3[] {
			new Vector3 (0, 0, 0),
			new Vector3 (scale.x, 0, 0),
			new Vector3 (scale.x, 0, scale.y),
			new Vector3 (0, 0, scale.y)
		};

		mesh.triangles = new int[] {
			2, 1, 0,
			0, 3, 2
		};

		mesh.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (textureScale.x, 0),
			new Vector2 (textureScale.x, textureScale.y),
			new Vector2 (0, textureScale.y)
		};

		quad.AddComponent<MeshRenderer> ().material.mainTexture = texture;

		mesh.RecalculateNormals(); 
		mesh.RecalculateBounds (); 
		mesh.Optimize();

		return quad;
	}

	//http://wiki.unity3d.com/index.php/ProceduralPrimitives
	protected GameObject ConeMesh(bool isCylinder, Vector3 scale, Texture texture, float textureScale, string tag)
	{
		GameObject cone = new GameObject ();
		MeshFilter filter = cone.AddComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();

		float height = scale.y;
		float bottomRadius = 0.5f*scale.x;
		float topRadius = .05f;
		if (isCylinder)
			topRadius = bottomRadius;
		int nbSides = 18;
		int nbHeightSeg = 1; // Not implemented yet

		int nbVerticesCap = nbSides + 1;
		#region Vertices

		// bottom + top + sides
		Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + nbSides * nbHeightSeg * 2 + 2];
		int vert = 0;
		float _2pi = Mathf.PI * 2f;

		// Bottom cap
		vertices[vert++] = new Vector3(0f, 0f, 0f);
		while( vert <= nbSides )
		{
			float rad = (float)vert / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
			vert++;
		}

		// Top cap
		vertices[vert++] = new Vector3(0f, height, 0f);
		while (vert <= nbSides * 2 + 1)
		{
			float rad = (float)(vert - nbSides - 1)  / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
			vert++;
		}

		// Sides
		int v = 0;
		while (vert <= vertices.Length - 4 )
		{
			float rad = (float)v / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
			vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0, Mathf.Sin(rad) * bottomRadius);
			vert+=2;
			v++;
		}
		vertices[vert] = vertices[ nbSides * 2 + 2 ];
		vertices[vert + 1] = vertices[nbSides * 2 + 3 ];
		#endregion

		#region Normales

		// bottom + top + sides
		Vector3[] normales = new Vector3[vertices.Length];
		vert = 0;

		// Bottom cap
		while( vert  <= nbSides )
		{
			normales[vert++] = Vector3.down;
		}

		// Top cap
		while( vert <= nbSides * 2 + 1 )
		{
			normales[vert++] = Vector3.up;
		}

		// Sides
		v = 0;
		while (vert <= vertices.Length - 4 )
		{			
			float rad = (float)v / nbSides * _2pi;
			float cos = Mathf.Cos(rad);
			float sin = Mathf.Sin(rad);

			normales[vert] = new Vector3(cos, 0f, sin);
			normales[vert+1] = normales[vert];

			vert+=2;
			v++;
		}
		normales[vert] = normales[ nbSides * 2 + 2 ];
		normales[vert + 1] = normales[nbSides * 2 + 3 ];
		#endregion

		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];

		// Bottom cap
		int u = 0;
		uvs[u++] = new Vector2(0.5f, 0.5f);
		while (u <= nbSides)
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			u++;
		}

		// Top cap
		uvs[u++] = new Vector2(0.5f, 0.5f);
		while (u <= nbSides * 2 + 1)
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			u++;
		}

		// Sides
		int u_sides = 0;
		while (u <= uvs.Length - 4 )
		{
			float t = (float)u_sides / nbSides;
			uvs[u] = new Vector3(t, 1f);
			uvs[u + 1] = new Vector3(t, 0f);
			u += 2;
			u_sides++;
		}
		uvs[u] = textureScale*new Vector2(1.0f, 1.0f);
		uvs[u + 1] = textureScale*new Vector2(1f, 0f);
		#endregion 

		#region Triangles
		int nbTriangles = nbSides + nbSides + nbSides*2;
		int[] triangles = new int[nbTriangles * 3 + 3];

		// Bottom cap
		int tri = 0;
		int i = 0;
		while (tri < nbSides - 1)
		{
			triangles[ i ] = 0;
			triangles[ i+1 ] = tri + 1;
			triangles[ i+2 ] = tri + 2;
			tri++;
			i += 3;
		}
		triangles[i] = 0;
		triangles[i + 1] = tri + 1;
		triangles[i + 2] = 1;
		tri++;
		i += 3;

		// Top cap
		//tri++;
		while (tri < nbSides*2)
		{
			triangles[ i ] = tri + 2;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = nbVerticesCap;
			tri++;
			i += 3;
		}

		triangles[i] = nbVerticesCap + 1;
		triangles[i + 1] = tri + 1;
		triangles[i + 2] = nbVerticesCap;		
		tri++;
		i += 3;
		tri++;

		// Sides
		while( tri <= nbTriangles )
		{
			triangles[ i ] = tri + 2;
			triangles[ i+1 ] = tri + 1;
			triangles[ i+2 ] = tri + 0;
			tri++;
			i += 3;

			triangles[ i ] = tri + 1;
			triangles[ i+1 ] = tri + 2;
			triangles[ i+2 ] = tri + 0;
			tri++;
			i += 3;
		}
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		cone.AddComponent<MeshRenderer> ().material.mainTexture = texture;

		mesh.RecalculateBounds();
		mesh.Optimize();

//		position.x += bottomRadius;
//		position.z += bottomRadius;
//		cone.transform.Translate (position);

		return cone;
	}

	// https://github.com/mortennobel/ProceduralMesh/blob/master/SimpleProceduralCube.cs
	protected GameObject BoxMesh(Vector3 scale, Texture texture, float textureScale, string tag)
	{
		GameObject box = new GameObject ();
		MeshFilter meshFilter = box.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh ();
		meshFilter.mesh = mesh;

		mesh.vertices = new Vector3[]{
			// face 1 (xy plane, z=0)
			new Vector3(0,0,0), 
			new Vector3(scale.x,0,0), 
			new Vector3(scale.x,scale.y,0), 
			new Vector3(0,scale.y,0), 
			// face 2 (zy plane, x=1)
			new Vector3(scale.x,0,0), 
			new Vector3(scale.x,0,scale.z), 
			new Vector3(scale.x,scale.y,scale.z), 
			new Vector3(scale.x,scale.y,0), 
			// face 3 (xy plane, z=1)
			new Vector3(scale.x,0,scale.z), 
			new Vector3(0,0,scale.z), 
			new Vector3(0,scale.y,scale.z), 
			new Vector3(scale.x,scale.y,scale.z), 
			// face 4 (zy plane, x=0)
			new Vector3(0,0,scale.z), 
			new Vector3(0,0,0), 
			new Vector3(0,scale.y,0), 
			new Vector3(0,scale.y,scale.z), 
			// face 5  (zx plane, y=1)
//			position+new Vector3(0,scale.y,0), 
//			position+new Vector3(scale.x,scale.y,0), 
//			position+new Vector3(scale.x,scale.y,scale.z), 
//			position+new Vector3(0,scale.y,scale.z), 
			// face 6 (zx plane, y=0)
//			position+new Vector3(0,0,0), 
//			position+new Vector3(0,0,scale.z), 
//			position+new Vector3(scale.x,0,scale.z), 
//			position+new Vector3(scale.x,0,0), 
		};

		int faces = 4; // here a face = 2 triangles

		List<int> triangles = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (int i = 0; i < faces; i++) {
			int triangleOffset = i*4;
			triangles.Add(0+triangleOffset);
			triangles.Add(2+triangleOffset);
			triangles.Add(1+triangleOffset);

			triangles.Add(0+triangleOffset);
			triangles.Add(3+triangleOffset);
			triangles.Add(2+triangleOffset);

			// same uvs for all faces
//			uvs.Add(new Vector2(0,0));
//			uvs.Add(new Vector2(1,0));
//			uvs.Add(new Vector2(1,1));
//			uvs.Add(new Vector2(0,1));
		}

		//badbadbad
		Vector3 texScale = scale * textureScale;
		// use passed textureScale
		uvs.Add (new Vector2 (0.0f, 0.0f));
		uvs.Add (new Vector2 (texScale.x, 0.0f));
		uvs.Add (new Vector2 (texScale.x, texScale.y));
		uvs.Add (new Vector2 (0.0f, texScale.y));

		uvs.Add (new Vector2 (0.0f, 0.0f));
		uvs.Add (new Vector2 (texScale.z, 0.0f));
		uvs.Add (new Vector2 (texScale.z, texScale.y));
		uvs.Add (new Vector2 (0.0f, texScale.y));

		uvs.Add (new Vector2 (0.0f, 0.0f));
		uvs.Add (new Vector2 (texScale.x, 0.0f));
		uvs.Add (new Vector2 (texScale.x, texScale.y));
		uvs.Add (new Vector2 (0.0f, texScale.y));

		uvs.Add (new Vector2 (0.0f, 0.0f));
		uvs.Add (new Vector2 (texScale.z, 0.0f));
		uvs.Add (new Vector2 (texScale.z, texScale.y));
		uvs.Add (new Vector2 (0.0f, texScale.y));

		mesh.triangles = triangles.ToArray();

		mesh.uv = uvs.ToArray();

		box.AddComponent<MeshRenderer> ().material.mainTexture = texture;

		// Normals ok?
		mesh.RecalculateNormals(); 
		mesh.RecalculateBounds (); 
		mesh.Optimize();

		return box;
		//renderer.material = new Material(Shader.Find("Diffuse"));
	}

	// only half the length normals on top?
	protected GameObject BoxMeshBrokenNormals(Vector3 scale, Vector3 position, Texture texture, Vector2 textureScale, string tag)
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
			
//		Vector3[] normals = new Vector3[] {
//			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
//			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
//			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
//			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
//			Vector3.up, Vector3.up, Vector3.up, Vector3.up
//		};

		mesh.vertices = moreVertices;
		mesh.uv = UVs;
		// only half the lenght on top?
		//mesh.normals = normals;

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

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		//mesh.uv = new Vector2[] { new Vector2 (0, 0), new Vector2 (0.5f*textureScale.x, textureScale.y), new Vector2 (textureScale.x, 0) };
		box.AddComponent<MeshRenderer> ().material.mainTexture = texture;

		return box;
	}

	protected GameObject TriangleMesh(Vector3[] positions, bool reversed, Texture roofTex, Vector2 texScale)
	{
		GameObject triangle = new GameObject ();
		Mesh mesh = new Mesh ();
		triangle.AddComponent<MeshFilter> ().mesh = mesh;

		mesh.vertices = positions;
		//triangle.transform.RotateAround(// (0.0f, 90.0f, 0.0f);
		if (reversed) {
			mesh.triangles = new int[] { 2, 1, 0 };
		} else {
			mesh.triangles = new int[] { 0, 1, 2 };
		}
		mesh.uv = new Vector2[] { new Vector2 (0, 0), new Vector2 (0.5f*texScale.x, texScale.y), new Vector2 (texScale.x, 0) };
		triangle.AddComponent<MeshRenderer> ().material.mainTexture = roofTex;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		return triangle;
	}

	// Update is called once per frame
	void Update () {

	}
}
