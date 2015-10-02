﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HealtComponent))]
public class Enemy : Unit {

	public enum State { Targeting, Engaging, Attacking, Death };
	State state;

	// Required componente
	HealtComponent _healthcomponent;

    public Transform target = null;
	public GameObject enemyProjectile;

	float attackRange = 0.8f;
	float rAttackRange = 4f;
	float range;
	bool isRanged;
	
	bool attacked = false;

	float xVel;
	float yVel;
	Vector2 randomVector;

	void Awake() 
	{
		_healthcomponent = GetComponent<HealtComponent>();
	}

	public override void Start() 
	{
		state = State.Targeting;

		// Temporary
		// Make enemy melee or ranged at random
		int meleeOrRanged = Random.Range(0, 2);
		
		if (meleeOrRanged == 0)
			isRanged = false;
		else
			isRanged = true;

		// Change range depening on if enemy is ranged or not.
		if (isRanged)
			range = rAttackRange;
		else
			range = attackRange;

		// Make a random vector.
		randomVector =  new Vector2( Random.Range(-0.35f, 0.35f), Random.Range(-0.1f, 0.1f ) );

        base.Start();
	}

	void Update() // Only for Colins feel test
	{
		if ( GameObject.Find( "EnemyManager" ) == null )
		{
			if ( Vector3.Normalize( GameObject.Find( "Player" ).transform.position
			          - transform.position ).x < 0 && facingRight )
				Flip();
			else if ( Vector3.Normalize( GameObject.Find( "Player" ).transform.position
			                            - transform.position ).x >= 0 && !facingRight )
				Flip();
		}
	}

	public State UppdateAttention( Transform player, List<Enemy> enemies )
	{
		if ( _healthcomponent.GetHealth() <= 0 ) // Dont continue if dead
		{
			state = State.Death;
			return state;
		}

		if ( state == State.Targeting ) 
			Search( player, enemies );
		else if ( state == State.Engaging ) 
			Engage();
		else if ( state == State.Attacking ) 
			Attack();
		else if ( state == State.Death ) 
			Die();

		if (xVel != 1 && state != State.Attacking) 
			base.FlipCheck(xVel);

		return state;
	}

	void Search( Transform player, List<Enemy> enemies ) 
	{
		if ( this.target == null ) 
			SelectTarget( player, enemies );

		base.Move();
    }

	void SelectTarget( Transform player, List<Enemy> enemies ) 
	{

		foreach (Enemy e in enemies)
		{
			if ( e.target == null && e != this )
			{
				target = e.transform;
				e.target = transform;
				state = State.Engaging;
				return;
			}
		}

		target = player;
		state = State.Engaging;
	}

    void Engage() 
	{

		//If engaged to player
		if( target == null ) // Dont continue if target is missing
		{
			state = State.Targeting;
			return;
		}
		
		// Variables
		Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
		Vector2 targetPos = new Vector2(target.position.x, target.position.y);
		Vector2 myTarget = Vector2.zero;
		
		// Change target vector depending on if the player is to the right of left of me.
		// Left
		if (target.position.x < transform.position.x)
			myTarget = targetPos + new Vector2(range - 0.15f, 0f);
		// Right
		else if (target.position.x > transform.position.x)
			myTarget = targetPos - new Vector2(range - 0.15f, 0f);

		// Add a bit of randomness to the target.
		myTarget += randomVector;

		// Get target direction.
		Vector2 targetDirection = myTarget - myPos;
		float distance = targetDirection.magnitude;
		Vector2 dir = targetDirection.normalized;
		xVel = dir.x;
		yVel = dir.y;
		
		// Enemy movement.
		// If close enough, Attack
		if (distance < 0.2f && state != State.Attacking)
		{
			state = State.Attacking;

			// Set speed for animation. 
			base.SetSpeed(0);

			//Flip if not facing player.
			if (target.position.x < transform.position.x && base.facingRight)
				base.Flip();
			else if (target.position.x > transform.position.x && !base.facingRight)
				base.Flip();
		}
		else // Move
		{
			transform.Translate(dir * 3f * Time.deltaTime);
			base.SetSpeed(1);
		}

    }

	void Attack() 
	{

		if (!attacked && isRanged)
		{
			attacked = true;
			StartCoroutine("RangedAttack");
		}
		else if (!attacked)
		{
			attacked = true;
			StartCoroutine("MeleeAttack");
		}


	}

	IEnumerator RangedAttack()
	{
		// Wait while Locking On
		yield return new WaitForSeconds(0.8f);

		// Shoot
		base.AttackAnim();
		GameObject spawnedProjectile = (GameObject)Instantiate(enemyProjectile, transform.position, Quaternion.identity);
		if (base.facingRight)
			spawnedProjectile.gameObject.GetComponent<EnemyProjectile>().MoveRight(true);
		else
			spawnedProjectile.gameObject.GetComponent<EnemyProjectile>().MoveRight(false);

		// Recover
		yield return new WaitForSeconds(0.5f);
		state = State.Engaging;
		attacked = false;
	}

	IEnumerator MeleeAttack()
	{
		base.AttackAnim(); // Attack
		
		yield return new WaitForSeconds(1f); // Recover
		attacked = false;
		state = State.Engaging;
	}

    void Die() 
	{
        state = State.Death;
		GameManager.instance.IncreaseMoney( 10 );
    }
	


}
