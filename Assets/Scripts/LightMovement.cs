using UnityEngine;
using System.Collections;

public class LightMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.rotation.eulerAngles.x < 90.0f) {
			gameObject.transform.Rotate (1.0f, 0.0f, 0.0f);
		}
	}
}
