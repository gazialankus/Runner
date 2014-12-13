using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldScroller : MonoBehaviour {

	public List<Transform> thingsToScroll;

	public Transform player;

	public float maxPlayerMoveDistance = 100;

	Vector3 playerInitialPosition;

	void Start() {
		playerInitialPosition = player.transform.position;
	}

	void Update () {
		if (Mathf.Abs(player.position.z - playerInitialPosition.z) > maxPlayerMoveDistance) {
			Scroll ();
		}
	}

	void Scroll () {
		Debug.Log ("will scroll");
		foreach (Transform t in thingsToScroll) {
			t.transform.Translate(0, 0, -maxPlayerMoveDistance, Space.World);
		}
	}
}
