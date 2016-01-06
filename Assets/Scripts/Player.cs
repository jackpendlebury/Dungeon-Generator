using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

	public int damage = 1;
	public int pointsPerhealth = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int health;
	private Text healthText; 

	// Use this for initialization
	protected override void Start () {

		animator = GetComponent<Animator> ();

		health = GameManager.instance.playerHealthPoints;
		healthText = GameManager.instance.healthText;

		healthText.text = "Health: " + health;
		base.Start ();
	}

	private void OnDisable(){
		GameManager.instance.playerHealthPoints = health;
	}

	protected override void OnCantMove <T> (T component){

		Wall hitWall = component as Wall;
		hitWall.DamageWall (damage);

		Enemy hitEnemy = component as Enemy;
		hitEnemy.TakeDamage(damage);

//		animator.SetTrigger ("PlayerChop");

	}

	private void OnTriggerEnter2D(Collider2D other){

		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			health += pointsPerhealth;
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
			healthText.text = "+" + pointsPerhealth + " Health: " + health;
			other.gameObject.SetActive (false);

		} else if (other.tag == "Soda") {
			health += pointsPerSoda;
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
			healthText.text = "+" + pointsPerSoda + " Health: " + health;
			other.gameObject.SetActive (false);
		}
		//TODO: Add Item Uses here (Gold, Health Etc)
	}

	private void Restart(){
		Application.LoadLevel (Application.loadedLevel);
	}

	private void CheckifGameOver(){
		if (health <= 0) {
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver();
		}
	}

	public void LoseHealth(int loss){
		animator.SetTrigger ("PlayerHit");
		health -= loss;
		healthText.text = "-" + loss + " Health: " + health;
		CheckifGameOver ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)(Input.GetAxisRaw ("Horizontal"));
		vertical = (int)(Input.GetAxisRaw ("Vertical"));

		if (horizontal != 0)
			vertical = 0;

		if (vertical > 0)
		{
			animator.SetInteger("Direction", 2);
		}
		else if (vertical < 0)
		{
			animator.SetInteger("Direction", 0);
		}
		else if (horizontal > 0)
		{
			animator.SetInteger("Direction", 1);
		}
		else if (horizontal < 0)
		{
			animator.SetInteger("Direction", 3);
		}

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall> (horizontal, vertical);
			AttemptMove<Enemy> (horizontal, vertical);
		}
	}

	protected override void AttemptMove<T> (int xDir, int yDir){
		healthText.text = "Health: " + health;
		base.AttemptMove<T> (xDir, yDir);
		RaycastHit2D hit;

		if(Move (xDir, yDir, out hit)){
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		CheckifGameOver ();
		GameManager.instance.playersTurn = false;
	}
}
