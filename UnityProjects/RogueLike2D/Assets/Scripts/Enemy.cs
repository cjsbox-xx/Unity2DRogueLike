using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {
	public int playerDamage;
	public AudioClip attack1Clip;
	public AudioClip attack2Clip;
	private Animator animator;
	private Transform target;
	private bool skipMove;
	// Use this for initialization
	protected override void Start () 
	{
		GameManager.instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start();
	}


	protected override void AttemptMove<T>(int dirX, int dirY)
	{
		if (skipMove)
		{
			skipMove = false;
			return;
		}
		base.AttemptMove<T>(dirX, dirY);
		skipMove = true;
	}

	public void MoveEnemy()
	{
		int dirX = 0;
		int dirY = 0;
		if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
		{
			dirY = target.position.y > transform.position.y ? 1 : -1;
		}
		else
		{
			dirX = target.position.x > transform.position.x ? 1 : -1;
		}
		AttemptMove<Player>(dirX, dirY);
	}

	protected override void OnCantMove<T> (T component)
	{
		Player hitPlayer = component as Player;

		animator.SetTrigger("enemyAttack");

		hitPlayer.LoseFood(playerDamage);

		SoundManager.instance.RandomizeSfx(attack1Clip, attack2Clip);
	}
}
