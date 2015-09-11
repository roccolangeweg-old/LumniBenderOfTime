using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    private bool paused;
    private float currentScale;
    private float healthMultiplier;

    private Canvas pauseScreen;

    /* stuff to save */
    private int collectedOrbs;
    private int totalOrbs;

    private int totalRelics;
    private int collectedRelics;
   
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
        healthMultiplier = 1;

        totalOrbs = PlayerPrefs.GetInt("Orbs");
        totalRelics = PlayerPrefs.GetInt("Relics");
	}

    void OnLevelWasLoaded() {
        if (Application.loadedLevelName == "Gamescreen") {
            pauseScreen = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
            collectedOrbs = 0;
            collectedRelics = 0;
        }

        if (Application.loadedLevelName == "ScoreScene") {
            totalOrbs += collectedOrbs;
            totalRelics += collectedRelics;
            PlayerPrefs.SetInt("Orbs", totalOrbs);
            PlayerPrefs.SetInt("Relics", totalRelics);

            PlayerPrefs.Save();
        }
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
            } else if (Application.loadedLevelName == "Gamescreen" && paused) {
                Time.timeScale = 1;
                Application.LoadLevel("MainMenu");
            } else if (Application.loadedLevelName == "Gamescreen" && !paused) {
                UpdatePauseState();
            }
        }

        if (Input.GetKeyDown(KeyCode.A)) {

            if(Time.timeScale != currentScale) {
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
        Time.timeScale = 1;
        Application.LoadLevel("MainMenu");
    }

    public float getHealthMultiplier() {
        return healthMultiplier;
    }

    public void addOrbs(int amount) {
        collectedOrbs += amount;
    }

    public int getCollectedOrbs() {
        return collectedOrbs;
    }

    public void UpdatePauseState() {
        paused = !paused;
        pauseScreen.enabled = !pauseScreen.enabled;
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