﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    private bool paused;
    private float currentScale;

    //Awake is always called before any Start functions
    void Awake() {
        /* check if gamemanager already exists */
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            /* enforce Singleton, only 1 game manager may exist */
            Destroy(gameObject);
        }    
            
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
           
	// Use this for initialization
	void Start () {
        currentScale = Time.timeScale;
	}
	
	// Update is called once per frame
	void Update () {
	
        if (Application.loadedLevelName == "MainMenu" && Input.GetMouseButtonDown(0)) {
            Application.LoadLevel("Gamescreen");
            paused = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Application.loadedLevelName == "MainMenu") {
                Application.Quit();
            } else {
                updatePauseState();
            }
        }

        if (Input.GetKeyDown(KeyCode.A)) {

            if(Time.timeScale != currentScale) {
                Debug.Log("SPEED BACK UP");
                /* speed up the game back to the previous timescale */
                StartCoroutine(updateTimescale(Time.timeScale, currentScale));
            } else {
                /* slow down the game to defined timescale */
                currentScale = Time.timeScale;
                StartCoroutine(updateTimescale(Time.timeScale, 0.6f));
            }

        }
	}

    public void playerDied() {
        Debug.Log("GAME OVER");
        Application.LoadLevel("MainMenu");
    }

    private void updatePauseState() {
        paused = !paused;
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    IEnumerator updateTimescale(float currentScale, float targetScale) {


        if (currentScale < targetScale) {
            while (currentScale < targetScale) {
                currentScale += 0.1f;
                Time.timeScale = currentScale;
                yield return new WaitForEndOfFrame();
            }
        } else {
            while (currentScale > targetScale) {
                currentScale -= 0.1f;
                Time.timeScale = currentScale;
                yield return new WaitForEndOfFrame();
            }
        }

    }
}