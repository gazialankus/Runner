using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	public Transform behindCameraStarts;

	public GameObject[] levelPiecePrefabs;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool isCreateWanted = false;
		Vector3 currentFrontMostPoint = new Vector3(0, 0, float.NegativeInfinity);
		foreach (Transform child in transform) {
			LevelPiece levelPiece = child.GetComponent<LevelPiece>();
			if (levelPiece != null) {
				Vector3 pieceFrontMostPoint = levelPiece.getFrontMostPoint();
				if (pieceFrontMostPoint.z < behindCameraStarts.position.z) {
					Debug.Log("level is behind: " + child.name);
					Destroy(child.gameObject);
					if (isCreateWanted) {
						Debug.LogError("There must be a mistake. Expected only one level left behind.");
					}
					isCreateWanted = true;
				} else if (pieceFrontMostPoint.z > currentFrontMostPoint.z) {
					currentFrontMostPoint = pieceFrontMostPoint;
				}
			}
		}

		if (isCreateWanted) {
			int chosenIndex = Random.Range(0, levelPiecePrefabs.Length);
			GameObject createdPiece = GameObject.Instantiate(levelPiecePrefabs[chosenIndex]) as GameObject;
			createdPiece.transform.parent = transform;
			createdPiece.transform.localPosition = Vector3.zero;

			LevelPiece levelPiece = createdPiece.GetComponent<LevelPiece>();
			levelPiece.calculateLocalFrontMostPoint();
			float backMostZ = levelPiece.getFrontMostPoint().z - levelPiece.getLength();
			Vector3 v = levelPiece.getFrontMostPoint();
			v.z = backMostZ;
			levelPiece.transform.Translate(0, 0, currentFrontMostPoint.z - backMostZ + 1, Space.World);
		}
	}
}
