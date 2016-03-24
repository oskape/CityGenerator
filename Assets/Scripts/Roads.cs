using UnityEngine;
using System.Collections;

public class Roads : Build {
	
	private float streetWidth;
	private GameObject intersection;
	private GameObject stretch;
	private Texture roadTexture;

	public void InitRoads(float aStreetWidth, Texture aRoadTexture, Texture aIntersectionTexture)
	{

		streetWidth = aStreetWidth;

		roadTexture = aRoadTexture;

		intersection = QuadMesh (new Vector2 (aStreetWidth, aStreetWidth), aIntersectionTexture, new Vector2 (1.0f, 1.0f), "Road");
		intersection.SetActive (false);
	}

	public GameObject[] RoadSection(Vector2 plotScale)
	{
		GameObject[] segments = new GameObject[4];

		Vector3 segmentPosition = new Vector3(-streetWidth, 0.01f, -streetWidth);

		// bottom left corner
		segments [0] = (GameObject)Instantiate(intersection, segmentPosition, Quaternion.identity);
		segments [0].SetActive (true);
		//		segments [0] = QuadMesh (segmentScale, interTex, textureScale, "Road");
		//		segments [0].transform.Translate (segmentPosition);

		// bottom right corner
		segmentPosition.x += streetWidth + plotScale.x;
		segmentPosition.y += 0.01f;
		segments [1] = (GameObject)Instantiate (intersection, segmentPosition, Quaternion.identity);
		segments [1].SetActive (true);
		//		segments [1] = QuadMesh (segmentScale, interTex, textureScale, "Road");
		//		segments [1].transform.Translate (segmentPosition);

		// along left
		segmentPosition = new Vector3(-streetWidth, 0, 0);
		Vector2 segmentScale = new Vector2(streetWidth, plotScale.y);
		Vector2 textureScale = new Vector2(1.0f, segmentScale.y);
		segments [2] = QuadMesh (segmentScale, roadTexture, textureScale, "Road");
		segments [2].transform.Translate (segmentPosition);
		// worth reconsidering instantiating if switching to segmented roadblocks rather than full sections (scaling is messy otherwise)
		//segments [2] = (GameObject)Instantiate (stretch, segmentPosition, Quaternion.identity);

		segmentPosition.x += segmentScale.x;
		segmentScale.y = plotScale.x;
		textureScale.y = segmentScale.y;
		segments [3] = QuadMesh (segmentScale, roadTexture, textureScale, "Road");
		segments [3].transform.Translate (segmentPosition);
		segments [3].transform.Rotate (0.0f, 90.0f, 0.0f);
		//segments [3] = (GameObject)Instantiate (stretch, segmentPosition, Quaternion.Euler (0.0f, 90.0f, 0.0f));

		return segments;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
