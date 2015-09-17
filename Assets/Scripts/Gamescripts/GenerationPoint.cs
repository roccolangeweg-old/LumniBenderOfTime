using UnityEngine;
using System.Collections;

public class GenerationPoint : MonoBehaviour {

    private LevelManager levelManager;
    private int levelDistance;
    private Vector3 startLevelPosition;

    private bool inTransition;

	// Use this for initialization
	void Start () {
        levelManager = FindObjectOfType<LevelManager>();
        levelDistance = levelManager.RandomLevelDistance();

        startLevelPosition = transform.position;
        inTransition = false;
	}
	
	// Update is called once per frame
	void Update () {
	    
        if (startLevelPosition.x + levelDistance < transform.position.x && !inTransition) {
            levelManager.StopSpawns();
            inTransition = true;
        }

	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "EndOfTunnel") {     

            levelManager.LoadNewLevel();
            inTransition = false;
            startLevelPosition = transform.position;
            levelDistance = levelManager.RandomLevelDistance();

        }
    } 
}
