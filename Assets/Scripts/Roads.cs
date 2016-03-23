using UnityEngine;
using System.Collections;

public class Roads : Build {

	private GameObject intersection;
	private GameObject stretch;

	public void InitIntersection(Vector2 scale, Vector2 textureScale, Texture texture)
	{
		intersection = QuadMesh (scale, texture, textureScale, "Road");
		intersection.SetActive (false);
	}

	// not used
	public void InitStretch(Vector2 scale, Vector2 textureScale, Texture texture)
	{
		stretch = QuadMesh (scale, texture, textureScale, "Road");
		stretch.SetActive (false);
	}

	public GameObject[] RoadSection(Vector2 plotScale, float streetSpace, Texture roadTex, Texture interTex)
	{
		GameObject[] segments = new GameObject[4];

		Vector3 flatRotation = new Vector3(90.0f, 0.0f, 0.0f);
		Vector3 segmentPosition = new Vector3(-streetSpace, 0.01f, -streetSpace);
		Vector2 segmentScale = new Vector3(streetSpace, streetSpace);
		Vector2 textureScale = new Vector2(1.0f, 1.0f);

		// bottom left corner
		segments [0] = (GameObject)Instantiate(intersection, segmentPosition, Quaternion.identity);
		segments [0].SetActive (true);
//		segments [0] = QuadMesh (segmentScale, interTex, textureScale, "Road");
//		segments [0].transform.Translate (segmentPosition);

		// bottom right corner
		segmentPosition.x += segmentScale.x + plotScale.x;
		segmentPosition.y += 0.01f;
		segments [1] = (GameObject)Instantiate (intersection, segmentPosition, Quaternion.identity);
		segments [1].SetActive (true);
//		segments [1] = QuadMesh (segmentScale, interTex, textureScale, "Road");
//		segments [1].transform.Translate (segmentPosition);

		// along left
		segmentScale = new Vector2(streetSpace, plotScale.y);
		segmentPosition = new Vector3(-streetSpace, 0, 0);
		textureScale = new Vector2(1.0f, segmentScale.y);
		segments [2] = QuadMesh (segmentScale, roadTex, textureScale, "Road");
		segments [2].transform.Translate (segmentPosition);
		// worth reconsidering instantiating if switching to segmented roadblocks rather than full sections (scaling is messy otherwise)
//		segments [2] = (GameObject)Instantiate (stretch, segmentPosition, Quaternion.identity);

		segmentScale = new Vector3(streetSpace, plotScale.x);
		segmentPosition.x += streetSpace;
		textureScale = new Vector2(1.0f, segmentScale.y);
		segments [3] = QuadMesh (segmentScale, roadTex, textureScale, "Road");
		segments [3].transform.Translate (segmentPosition);
		segments [3].transform.Rotate (0.0f, 90.0f, 0.0f);
//		segments [3] = (GameObject)Instantiate (stretch, segmentPosition, Quaternion.Euler (0.0f, 90.0f, 0.0f));

		return segments;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
