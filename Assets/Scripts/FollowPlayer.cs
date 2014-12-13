using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Transform player;

	Vector3 positionWrtPlayer;

	void Start () {
		positionWrtPlayer = transform.position - player.position;
	}

	void Update () {
		transform.Translate(0, 0, player.position.z + positionWrtPlayer.z - transform.position.z);
	}

}
