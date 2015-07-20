using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct Coordinate {
	// Struct for block coordinates.

	public float x;
	public float y;

	public Coordinate(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public Coordinate(Coordinate coord) {
		x = coord.x;
		y = coord.y;
	}
}

public class GameController : MonoBehaviour {
		
	public const int NumLives = 3;
	private const int BaseBlocks = 5;
	public List<Coordinate> xyList = new List<Coordinate>();
	
	public Text uiText;
	public GameObject gameOverObject;
	public bool gameOver;
	public GameObject block;
	public GameObject ballPrefab;
	public GameObject paddle;
	public GameObject leftWall;
    public int level;
	public bool invincible = false;
	public List<GameObject> blocks;
	public bool countdown = false;

	private int score;
	private int lives;
	private int blocksRemaining;
	private float timer = 0;
	private bool largePaddle = false;
	private GameObject ball;

	// Helper variable so Update() does not continuously increase level on wait.
	private bool incrementing = false;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		initializeArray();
		reset();
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("escape")) {
			Application.LoadLevel ("Title");
		}

		// If you wish to reset game.
		if (gameOver && Input.GetKeyDown("space")) {
			reset();
		}

		// Incrementing of level code here.
		if (blocksRemaining == 0 && !incrementing) {
			write();
			StartCoroutine(resetPositions());
			spawnBlocks(++level);
		}

		/* 
		 * Timer function of update. If the timer > 0 will count down automatically
		 * and if it is less than zero, checks to remove the power-up.
		 */

		if (timer > 0 && countdown == false) {
			timer -= Time.deltaTime;
			if (timer <= 1 && largePaddle) {
				flickerReverse(paddle.GetComponent<SpriteRenderer>(), Color.black);
			} 
			
			if (timer <= 1 && invincible) {
				flickerReverse(leftWall.GetComponent<SpriteRenderer>(), Color.black);
			}
		}

		if (timer <= 0 && largePaddle) {
			compressPaddle();
			paddle.GetComponent<SpriteRenderer>().color = Color.white;
		}

		if (timer <= 0 && invincible) {
			invincible = false;
			leftWall.GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	private void spawnBlocks(int level) {


		// If there are bricks still remaining on the board, destroy all of them.
		if (blocks != null) {
			for (int i = 0; i < blocks.Count; i ++) {
				if (blocks[i] != null) {
					Object.Destroy(blocks[i]);
				}
			}
		}

		 // Spawn blocks on map using the XPositions and YPositions readonly arrays. 

		blocksRemaining = BaseBlocks + System.Math.Min(UnityEngine.Random.Range(level - 1, level), 45 - BaseBlocks);

		// Potential locations on board for bricks to be selected at random.
		List<Coordinate> chosen = choose(xyList, blocksRemaining);
		bool specialBlock = false;

		for (int i = 0; i < blocksRemaining; i ++){ 

			Vector2 spawnPosition = new Vector2(chosen[i].x, chosen[i].y);

			Quaternion spawnRotation = Quaternion.identity;
			GameObject newBlock = (GameObject) Instantiate(block, spawnPosition, spawnRotation);
			newBlock.GetComponent<SpriteRenderer>().enabled = false;
			blocks.Add(newBlock);

			if (Random.Range(0,10) == 0 && !specialBlock) {
				newBlock.GetComponent<BlockDestroy>().type = Random.Range(2,5);
				specialBlock = true;
				
			} else if (Random.Range(0,100) < (2 * level)) {
				newBlock.GetComponent<BlockDestroy>().type = 1;
			}

		}

		if (specialBlock) {
			blocksRemaining--;
		}
	}

	private List<Coordinate> choose(List<Coordinate> li, int size) {

		// Returns a list of length 'size' with elements chosen randomly from input list.
		
		List<Coordinate> newList = li.ConvertAll(l => new Coordinate(l));
		
		while (newList.Count > size) {
			newList.RemoveAt(Random.Range(0, newList.Count - 1));
		}
		
		return newList;
		
	}

	private void flickerReverse(SpriteRenderer sprite, Color color) {
		if (sprite.color == color) {
			sprite.color = Color.clear;
		} else {
			sprite.color = color;
		}
	}


	private void reset() {

		// Resets the game.
		gameOver = false; blocksRemaining = 0; score = 0; lives = NumLives; level = 0;
		gameOverObject.SetActive(false);
		write();

	}

	private void initializeArray() {

		// Initializes the array of positions.
		float[] xList = { -1f, .5f, 0f, .5f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f };
		float[] yList = { 3f, 1.5f, 0f, -1.5f, -3f };
		for (int i = 0; i < xList.Length; i ++) {
			for (int j = 0; j < yList.Length; j ++) {
				xyList.Add( new Coordinate(xList[i], yList[j]) );
			}
		}
	}

	private void write() {

		// Writes the score on the UI.
		string strLives = "";
		for (int i = 0; i < lives; i ++) {
			strLives += "*";
		}
		uiText.text = "Score: " + score + " | Lives: " + strLives + " | Level: " + level;
	}


	public void addScore(int addedScore, bool blockDestroyed) {

		// Adds score and determines whether a block was destroyed or not.
		if (blockDestroyed) {
			blocksRemaining --;
		}

		score += addedScore;
		write();
	}

	public void loseLife() {

		// Indicates when a player has lost a life.
		if (--lives <= 0) {
			endGame();
		}
		StartCoroutine(resetPositions());
		write();

	}

	public void expandPaddle() {

		// Expands paddle to a larger shape.
		paddle.transform.localScale = new Vector3(8,9,1);
		paddle.GetComponent<PlayerController>().largePaddle = true;
		timer = 5f;
		largePaddle = true;

	}

	private void compressPaddle() {

		// Compresses paddle back to its original size.
		paddle.transform.localScale = new Vector3(6f,6f,1f);
		paddle.GetComponent<PlayerController>().largePaddle = false;
		largePaddle = false;

    }

	
	public void invincibility() {

		// Turns on invinciblity for 5 seconds.
		invincible = true;
		timer = 5f;
		leftWall.GetComponent<SpriteRenderer>().enabled = true;

    }

	public void extraLife() {

		// Grants an extra life to the player.
		lives ++;

	}


	private IEnumerator resetPositions() {

		// Destroys ball and creates a new one.
		countdown = true;
		if (ball != null) {
			Object.Destroy(ball);
		}

		if (!gameOver) {
			ball = (GameObject) Instantiate(ballPrefab, new Vector2(-3,0), Quaternion.identity);
			yield return new WaitForSeconds(3);
			ball.GetComponent<Rigidbody2D>().velocity = new Vector2(-1f,0).normalized * 5;
			countdown = false;
		}

	}

	private void endGame() {

		// Ends game.
		gameOverObject.SetActive(true);
		gameOver = true;

	}
	
}
