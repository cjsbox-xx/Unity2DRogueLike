﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1.0f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;
	// Use this for initialization
	protected override void Start () 
	{
		animator = GetComponent<Animator>();
		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;
		base.Start();
	}

	private void OnDisable()
	{
		GameManager.instance.playerFoodPoints = food;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!GameManager.instance.playersTurn)
		{
			return;
		}
		int horizontal = 0;
		int vertical = 0;
		horizontal = (int)Input.GetAxisRaw("Horizontal");
		vertical = (int)Input.GetAxisRaw("Vertical");
		if (horizontal != 0)
		{
			vertical = 0;
		}
		if (horizontal != 0 || vertical != 0)
		{
			AttemptMove<Wall>(horizontal, vertical);
		}
	}

	protected override void AttemptMove<T>(int dirX, int dirY)
	{
		food--;
		foodText.text = "Food: " + food;
		base.AttemptMove<T>(dirX, dirY);

		RaycastHit2D hit;

		if (Move(dirX, dirY, out hit))
		{
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		CheckIfGameOver();

		GameManager.instance.playersTurn = false;
	}
		

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Exit")
		{
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}
		else if (other.tag == "Food")
		{
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			food += pointsPerFood;
			other.gameObject.SetActive(false);
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
		}
		else if (other.tag == "Soda")
		{
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			food += pointsPerSoda;
			other.gameObject.SetActive(false);
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
		}
	}

	protected override void OnCantMove<T>(T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall(wallDamage);
		animator.SetTrigger("playerChop");
	}

	private void Restart()
	{
		SceneManager.LoadScene(0);
	}

	public void LoseFood(int loss)
	{
		animator.SetTrigger("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food : " + food;
		CheckIfGameOver();
	}

	private void CheckIfGameOver()
	{
		if (food <= 0)
		{
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver();
		}
	}
}
