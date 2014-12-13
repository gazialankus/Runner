using UnityEngine;
using System.Collections;

public class LevelPiece : MonoBehaviour {

	//assuming no rotation on this object

	Vector3 localFrontmostPoint;
	float length;
	bool isLocalFrontmostPointValid;
	
	public void calculateLocalFrontMostPoint() {
		Bounds b = new Bounds();
		bool first = true;
		foreach (Transform child in transform) {
			if(child.renderer != null) {
				if (first) {
					b = child.renderer.bounds;
					first = false;
				} else {
					b.Encapsulate(child.renderer.bounds);
				}
			}
		}
		// from that, calculate frontmost point
		Vector3 frontMostPoint = b.center + b.extents;
		localFrontmostPoint = frontMostPoint - transform.position;
		isLocalFrontmostPointValid = true;
		length = b.extents.z * 2;
	}

	// Use this for initialization
	void Start () {
		if (!isLocalFrontmostPointValid) {
			calculateLocalFrontMostPoint();
		}
	}

	public Vector3 getFrontMostPoint() {
		return transform.position + localFrontmostPoint;
	}

	public float getLength() {
		return length;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
