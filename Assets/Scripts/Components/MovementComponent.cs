﻿using UnityEngine;
using System.Collections;

public class MovementComponent : MonoBehaviour {

	float speed = 5;

	public void Move( float horizontal, float vertical ) {
		transform.position = new Vector3( Mathf.Clamp( transform.position.x + horizontal * speed * Time.deltaTime, -8f, 8f ),
		                                 Mathf.Clamp( transform.position.y + vertical * speed * Time.deltaTime, -4f, 4f ),
		                                 0f );
	}

	public void ChangeSpeed( int speed )
	{
		if( gameObject.tag == "Player" )
			this.speed = speed;
	}

}
