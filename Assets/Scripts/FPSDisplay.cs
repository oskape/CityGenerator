using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	private Build builder;
	private float townWidth;
	private float townLength;

	void Start()
	{
		
	}

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	public void SetScale(float aWidth, float aLength)
	{
		townWidth = aWidth;
		townLength = aLength;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);

		Rect rect2 = new Rect(0, h * 2 / 100, w, h * 2 / 100);
		string text2 = string.Format ("{1:0} x {1:0}", townWidth, townLength);
		GUI.Label (rect2, text2, style);
	}
}