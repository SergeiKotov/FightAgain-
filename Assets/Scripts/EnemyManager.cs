﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	FileReader _fReader;

	List<string> enemyWaves;
	List<int> tempRange;
	int wave;
	bool spawning;
	public List<Enemy> enemies = new List<Enemy>();
	public SpawnPoint[] spawnPoints;

	float waveTime;
	float waveDuration = 10f;

	public Enemy enemy;

	public bool spawnOne;

	void Awake() 
	{
		tempRange = new List<int>();
		tempRange.Add( 1 );
		tempRange.Add( 2 );
		//tempRange.Add( 3 );
		//List<int> tempWave = GenerateWave( 10, tempRange );

		/*TextAsset wavesFile = Resources.Load( "waves" ) as TextAsset;
		List<string> fileLines = new List<string>();
		string [] linesFromFile = wavesFile.text.Split( "\n"[0] );
		
		foreach ( string line in linesFromFile ) {
			if( line != null )
				fileLines.Add( line );
		}
		enemyWaves = fileLines;*/

		//_fReader = new FileReader();
		//enemyWaves = _fReader.ReadWavesFile( "waves" );
		wave = 0;
		if ( spawnOne )
			enemies.Add( SpawnEnemy( 1 ) );
	}
    
    void Update() 
	{
		waveTime += Time.deltaTime;

		if( enemies.Count > 0 ) 
		{
			for( int i = 0; i < enemies.Count; i++ ) 
			{
				if( enemies[ i ].UppdateAttention( GameManager.instance.GetPlayerTransform(), enemies ) == Enemy.State.Death )
				{
					enemies[ i ].StopAllCoroutines();
					Destroy( enemies[ i ].gameObject );
					enemies.RemoveAt( i );
				}
			}

		} 
		else if(/* wave < enemyWaves.Count  && */spawning == false && !spawnOne ) 
		{
			List<int> tempList = GenerateWave( (wave+1)*8, tempRange );
			for ( int i = 0; i < tempList.Count; i++ ) {
				//Debug.Log( "tempList[" + i + "]: " + tempList[ i ] );
			}
			StartCoroutine( SpawnWave( tempList ) );
		}

    }

	List<int> GenerateWave( int resource, List<int> range )
	{
		List<int> waveList = new List<int>();

		do {
			int e = Random.Range( 0, range.Count );
			if( range[ e ] > resource )
			{
				//Debug.Log( "loool " + range[ e ] + " ... " + resource );
				waveList.Add( resource );
				//Debug.Log( "... " + resource );
				resource = 0;
				break;
			}
			waveList.Add( range[ e ] );
			//Debug.Log( "... " + range[ e ] );
			resource -= range[ e ];
			//Debug.Log( range[ e ] + " ... " + resource );
		} while( resource > 0 );

		//Debug.Log( "wave count= " + waveList.Count );
		for ( int i = 0; i < waveList.Count; i++ ) {
			//Debug.Log( "waveList value: " + waveList[ i ] );
		}
		//Debug.Log( "enemies in wave: " + waveList.Count );
		return waveList;
	}

	IEnumerator SpawnWave( List<int> waveList )
	{
		//if( this.wave < enemyWaves.Count -1 ) {
			waveTime = 0f;
			spawning = true; //TODO: Temp solution?
			int spawned = 0;
		//Debug.Log(3);
			//string wave = enemyWaves[ this.wave += 1 ];

			for( int i = 0; i < waveList.Count /* spawnPoints.Length*/; i++ ) 
			{
				yield return new WaitForSeconds( 1f ); // This breaks on level load for some reason
				for( int j = 0; j < spawnPoints.Length; j++ )
					if( waveList.Count > spawned )
					{
						enemies.Add( SpawnEnemy( j ) );
						spawned += 1;
					}
			}
			spawning = false;
		//}
	}

	Enemy SpawnEnemy( int point ) 
	{ // Do pooling for this
		return Instantiate( enemy, spawnPoints[ point ].getPos(), Quaternion.identity ) as Enemy;
	}

}
