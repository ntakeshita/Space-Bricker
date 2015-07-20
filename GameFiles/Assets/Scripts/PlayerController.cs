using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public GameController gameController;
	public int speed = 25;
	public bool largePaddle;
	

	// Use this for initialization
	void Start () {
		largePaddle = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float v = Input.GetAxisRaw("Vertical");

		if (gameController.gameOver == false && !largePaddle) {

			// Enforce paddle boundries on walls if not a large paddle.
			if (transform.position.y >= 3) {
				v = Mathf.Min(v, 0f);
			}
			if (transform.position.y <= -3) {
				v = Mathf.Max(v, 0f);
			}
		}

		if (gameController.gameOver == false && largePaddle) {

			// Enforce paddle boundries on walls if large paddle.
			if (transform.position.y >= 2.6) {
				v = Mathf.Min(v, 0f);
			}
			if (transform.position.y <= -2.6) {
				v = Mathf.Max(v, 0f);
			}
		}

		GetComponent<Rigidbody2D>().velocity = new Vector2(0,v) * speed;
	}
	
}
