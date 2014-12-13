using UnityEngine;
using System.Collections;

public class TerrainScroller : MonoBehaviour {

	public Transform[] terrains;

	public Transform previousTerrainPassedLocation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Transform backMostTerrain = null;
		float backMostTerrainZ = float.PositiveInfinity;
		bool frontTerrainExists = false;

		foreach (Transform terrain in terrains) {
			if (terrain.position.z > previousTerrainPassedLocation.position.z) {
				frontTerrainExists = true;
				break;
			}

			if (terrain.position.z < backMostTerrainZ) {
				backMostTerrainZ = terrain.position.z;
				backMostTerrain = terrain;
			}
		}

		if (!frontTerrainExists) {
			// assuming only two terrains by this subtraction
			backMostTerrain.Translate(0, 0, 2 * Mathf.Abs(terrains[0].position.z - terrains[1].position.z));
		}
	}
}
