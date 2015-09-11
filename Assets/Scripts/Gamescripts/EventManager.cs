using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    private GameManager gameManager;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setGamePaused() {
        gameManager.UpdatePauseState();
    }
}
