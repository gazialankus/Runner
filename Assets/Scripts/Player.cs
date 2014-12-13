using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//TODO actually make this move with a character controller
	//TODO jump
	//TODO states to be in. shold be able to undo a motion. should be able to break crouch/jump
	//TODO crouch. implement with scale for now. 

	public Lane currentLane;

	bool isSwitchingLanes;
	public float laneSwitchSpeed = 10;

	public Vector3 runningVelocity = new Vector3(0, 0, 10);
	Vector3 defaultRunningVelocity;
	bool isRunning = true;
	bool isJumping;
	bool isCrouching;

	public bool isReactingToHit;
	Vector3 reactToHitStartVelocity = new Vector3(0, 0, -10);
	Vector3 reactToHitAcceleration = new Vector3(0, 0, 40);

	Vector3 currentVerticalVelocity = Vector3.zero;
	public Vector3 jumpStartVerticalVelocity = new Vector3(0, 15, 0);

	public float gravityMultiplier = 4;

	CharacterController controller;

	float defaultControllerHeight;
	float crouchControllerHeight = 1f;

	void Awake () {
		controller = GetComponent<CharacterController>();
		defaultControllerHeight = controller.height;
	}

	// Use this for initialization
	void Start () {
		if (currentLane == null) {
			// Find the closest lane and choose it
			Lane[] lanes = GameObject.FindObjectsOfType<Lane>();

			float currentLaneDistance = float.PositiveInfinity;
			foreach (Lane lane in lanes) {
				float laneDistance = Vector3.Distance(transform.position, lane.transform.position);

				if (laneDistance < currentLaneDistance) {
					currentLane = lane;
					currentLaneDistance = laneDistance;
				}
			}
		} 

		// Move the player to the current lane
		transform.position = currentLane.transform.position;

		displayLives();
		displayScore();
	}
	
	void Update () {
		if (Input.GetButtonDown("Horizontal") || !isSwitchingLanes) {
			if(Input.GetAxis("Horizontal") < 0) {
				// Move left
				if(currentLane.leftLane != null) {
					moveToLane(currentLane.leftLane);
				} else {
					laneMoveMistake("left");
				}
			} else if(Input.GetAxis("Horizontal") > 0) {
				// Move right
				if(currentLane.rightLane != null) {
					moveToLane(currentLane.rightLane);
				} else {
					laneMoveMistake("right");
				}
			}
		}

		bool isCrouchWanted = false;

		if(Input.GetAxis("Vertical") < 0) {
			//Crouch
			isCrouchWanted = true;
		} else if(Input.GetAxis("Vertical") > 0) {
			//Jump
			if (!isJumping) {
				isJumping = true;
				currentVerticalVelocity = jumpStartVerticalVelocity;
			}
		}

		bool isMoveUsed = false;

		Vector3 moveVector = Vector3.zero;

		Vector3 cumulativeVelocity = Vector3.zero;
		if (isSwitchingLanes) {
			Vector3 vectorToTarget = currentLane.transform.position - transform.position;
			vectorToTarget.y = 0; //ignore vertical
			if (vectorToTarget.magnitude < laneSwitchSpeed * Time.deltaTime) {
				isMoveUsed = true;
				moveVector = vectorToTarget;
				isSwitchingLanes = false;
			} else {
				cumulativeVelocity += vectorToTarget.normalized * laneSwitchSpeed;
			}
		}

		if (isReactingToHit) {
			runningVelocity += reactToHitAcceleration * Time.deltaTime;
			if (runningVelocity.z > defaultRunningVelocity.z) {
				isReactingToHit = false;
				runningVelocity = defaultRunningVelocity;
			}
		}

		if (isRunning) {
			cumulativeVelocity += runningVelocity;
		}

		if (isJumping) {
			cumulativeVelocity += currentVerticalVelocity;
			isMoveUsed = true;

			float crouchGravityMultiplier = 1;
			if (isCrouchWanted) {
				crouchGravityMultiplier = 10;
			}
			currentVerticalVelocity += Physics.gravity * Time.deltaTime * gravityMultiplier * crouchGravityMultiplier; //multipliers for faster jumps 
		} else {
			if (isCrouchWanted) {
				isCrouching = true;
				transform.localScale = new Vector3(1, .5f, 1);
				controller.height = crouchControllerHeight;
			}
		}

		if (isCrouching && !isCrouchWanted) {
			transform.localScale = new Vector3(1, 1, 1);
			controller.height = defaultControllerHeight;
		}

		if (isMoveUsed) {
			controller.Move(moveVector + cumulativeVelocity * Time.deltaTime);
		} else {
			controller.SimpleMove(cumulativeVelocity);
		}

		if (controller.isGrounded) {
			isJumping = false;
		}
	}

	void moveToLane (Lane newLane)
	{
		isSwitchingLanes = true;
		currentLane = newLane;
	}

	void laneMoveMistake (string direction)
	{
		Debug.Log("mistake " + direction);
	}




	int score;
	int lives = 3;

	public GUIText scoreText;
	public GUIText livesText;
	public GUIText gameOverText;

	void displayScore()
	{
		scoreText.text = "Puan: " + score;
	}
	
	void displayLives()
	{
		livesText.text = "Can: " + lives;
	}
	
	void OnTriggerEnter(Collider other) {
		Pickable pickable = other.GetComponent<Pickable>();
		if (pickable != null) {
			score = score + pickable.value;
			displayScore ();
			pickable.gameObject.SetActive(false);
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.moveDirection.z > 0.9 && !isReactingToHit) {
			//hit sraight ahead
			Debug.Log("hit");
			lives = lives - 1;
			displayLives();
			if (lives == 0) {
				gameOver();
			}
			isReactingToHit = true;
			defaultRunningVelocity = runningVelocity;
			runningVelocity = reactToHitStartVelocity;
		}
	}

	void gameOver ()
	{
		gameOverText.gameObject.SetActive(true);
		Time.timeScale = 0;
	}
}
	
	