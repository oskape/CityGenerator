using UnityEngine;
using System.Collections;

// http://stackoverflow.com/questions/8465323/unity-fps-rotation-camera
// http://forum.unity3d.com/threads/looking-with-the-mouse.109250/
/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
//[AddComponentMenu("Camera-Control/Mouse Look")]
public class FlyCameraFPS : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 10F;
	public float sensitivityY = 10F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

	public float moveSpeed = 0.1f;
	public float boost = 5.0f;

	private Build builder;

	// Update is called once per frame
	void Start () {
		builder = GetComponent<Build> ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R)) {
			builder.CleanWorld ();
			builder.Init ();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			moveSpeed *= boost;
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			moveSpeed /= boost;
		}

		if (axes == RotationAxes.MouseXAndY && (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0))
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			//OLD WAY: transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivityY, Input.GetAxis("Mouse X") * sensitivityX, 0);
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}

		if (Input.GetKey(KeyCode.W))
		{
			Vector3 newPos = transform.position + moveSpeed * transform.forward;
			transform.position = new Vector3();
			transform.position = newPos;
			//float newPos = transform.position.z + 0.1f;
			//transform.position = new Vector3(transform.position.x, transform.position.y, newPos);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			Vector3 newPos = transform.position - moveSpeed * transform.forward;
			transform.position = new Vector3();
			transform.position = newPos;
			//float newPos = transform.position.z - 0.1f;
			//transform.position = new Vector3(transform.position.x, transform.position.y, newPos);
		}

		if (Input.GetKey(KeyCode.A))
		{
			Vector3 newPos = transform.position - moveSpeed * transform.right;
			transform.position = new Vector3();
			transform.position = newPos;
			//float newPos = transform.position.x - 0.1f;
			//transform.position = new Vector3(newPos, transform.position.y, transform.position.z);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			Vector3 newPos = transform.position + moveSpeed * transform.right;
			transform.position = new Vector3();
			transform.position = newPos;
			//float newPos = transform.position.x + 0.1f;
			//transform.position = new Vector3(newPos, transform.position.y, transform.position.z);
		}

		if (Input.GetKey(KeyCode.Q))
		{
			Vector3 newPos = transform.position + moveSpeed * transform.up;
			transform.position = new Vector3();
			transform.position = newPos;
			//float newPos = transform.position.y + 0.1f;
			//transform.position = new Vector3(transform.position.x, newPos, transform.position.z);
		}
		else if (Input.GetKey(KeyCode.E))
		{
			Vector3 newPos = transform.position - moveSpeed * transform.up;
			transform.position = new Vector3();
			transform.position = newPos;
			//float newPos = transform.position.y - 0.1f;
			//transform.position = new Vector3(transform.position.x, newPos, transform.position.z);
		}

		if (Input.GetKey(KeyCode.B))
		{
			transform.position = GameObject.FindWithTag("Building").transform.position;
		}
		if (Input.GetKey(KeyCode.U))
		{
			transform.up = Vector3.up;
		}
	}
}
