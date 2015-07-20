using System;
using UnityEngine;
using System.Collections;

public class BlockDestroy : MonoBehaviour {

	public GameController gameController;
	private int strength = 1;
	private int health = 1;
	public int type = 0;

	/* 
	 * 'type' of block defines the type of brick based on its int value
	 * - 0: Regular Brick = Red
	 * - 1: Tough Brick = Black
	 * - 2: Power-Up: Invincibility = Blue
	 * - 3: Power-Up: Paddle Expansion = Yellow
	 * - 4: Extra Life = Green
	 */

	void Start() {

		// Finding the GameController.
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		} else {
			Debug.Log ("Can't find GameController object.");
			return;
		}

		// Setting the type of block.
		if (type == 1) {
			gameObject.GetComponent<SpriteRenderer>().color = Color.black;
			strength = health = Math.Min(gameController.level + 1, 5);
		} else if (type == 2) {
			gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
		} else if (type == 3) {
			gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
		} else if (type == 4) {
			gameObject.GetComponent<SpriteRenderer>().color = Color.green;
		}

	}

	void Update() {
		if (!gameController.countdown) {
			gameObject.GetComponent<SpriteRenderer>().enabled = true;
		}
	}
	

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Ball") {
			bool destroyed = false;
			health --;
			if (health == 0) {
				UnityEngine.Object.Destroy(this.gameObject);
				gameController.blocks.Remove(this.gameObject);
				if (type < 2) {
					destroyed = true;
				}
				if (type == 2) {
					gameController.invincibility();
				}
				if (type == 3) {
					gameController.expandPaddle();
				}
				if (type == 4) {
					gameController.extraLife();
				}
			}
			gameController.addScore(strength * (strength + 1 - health), destroyed);
		}
	}
	
}
