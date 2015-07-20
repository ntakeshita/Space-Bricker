using UnityEngine;
using System.Collections;

public class WallCheck : MonoBehaviour {

	public GameObject ball;
	public GameObject paddle;
	public GameController gameController;

	void Start() {
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}

	// Update is called once per frame
	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Ball" && !gameController.invincible) {
			/* Code here */

			gameController.loseLife();

		}
	}
}
