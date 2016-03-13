using UnityEngine;
using System.Collections;

public class Roads : Build {

//	public Texture roadTex;
//	public Texture interTex;

	public GameObject[] RoadSection(Vector3 scale, Vector3 position, float streetSpace, Texture roadTex, Texture interTex)
	{
		GameObject[] segments = new GameObject[4];

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

		return segments;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
