﻿using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour {
	
	bool moveRight = true;
	float lifeTime;

	void Update () 
	{
		lifeTime += Time.deltaTime;
		if (lifeTime > 10f)
			Destroy(gameObject);

		if (moveRight)
			transform.position += transform.right * 768f * Time.deltaTime;
		else
			transform.position -= transform.right * 768f * Time.deltaTime;
	}

	public void MoveRight (bool right)
	{
		if (right)
			moveRight = true;
		else
			moveRight = false;
	}


	// prolly don't need this since we have damage component
	/*void OnTriggerEnter2D (Collider2D other)
	{
		if ( other.gameObject.tag == "Player" )
		{
			other.gameObject.SendMessage( "TakeDamage" , damage , SendMessageOptions.DontRequireReceiver );
			Destroy(gameObject);
		}
	}*/
}
