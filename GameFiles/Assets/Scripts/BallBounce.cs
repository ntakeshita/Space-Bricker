using UnityEngine;
using System.Collections;

public class BallBounce : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("MidWall").GetComponent<Collider2D>());
	}
		
	void Update() {
		Vector3 velocity = GetComponent<Rigidbody2D>().velocity;
		if (isBetween(velocity.x, -.5f, .5f) && velocity.y > 5) {
			Debug.Log ("changed velocity");
			GetComponent<Rigidbody2D>().velocity = new Vector2(-1, choose(-1f, 1f));
		}
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.gameObject.tag == "Paddle") {
			float angle = (this.transform.position.y - other.transform.position.y) / other.collider.bounds.size.y;
			Vector2 v = new Vector2(1, angle);
			GetComponent<Rigidbody2D>().velocity = v.normalized * speed;
		}
		                                     

	}

	bool isBetween(float f, float low, float high) {
		return low < f && f < high;
	}

	float choose(float f1, float f2) {
		if (Random.Range (0,1) > .5) {
			return f1;
		}
		return f2;
	}

}
