using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSpawn : MonoBehaviour {

    public GameObject BirdPrefab;
    public GameObject Player;
    public Vector3 BirdSpawnPos = Vector3.zero;

    private Vector3 SpawnPos;

	// Use this for initialization
	void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
        SpawnPos = Player.transform.TransformPoint(BirdSpawnPos);
	}
	
	// Update is called once per frame
	void Update () {

        SpawnPos = Player.transform.TransformPoint(BirdSpawnPos);

        //test spawn code
        if (Input.GetButtonDown("Fire1"))
        {
            Spawn();
        }



	}

    void Spawn()
    {
        Instantiate(BirdPrefab, SpawnPos, Quaternion.LookRotation(Player.transform.position));
    }
}
