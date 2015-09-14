using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager managerInstance = null;

    private GoogleAnalyticsV3 GA3;

    private bool paused;
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
        if (managerInstance == null) {
            managerInstance = this;
        } else if (managerInstance != this) {
            /* enforce Singleton, only 1 game manager may exist */
            Destroy(gameObject);
        }    
        GA3 = FindObjectOfType<GoogleAnalyticsV3>();
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
           
	// Use this for initialization
	void Start () {
        healthMultiplier = 1;

        GA3.LogEvent("MainMenu","Main","Application Loaded",1);

        totalOrbs = PlayerPrefs.GetInt("Orbs");
        totalRelics = PlayerPrefs.GetInt("Relics");
	}

    void OnLevelWasLoaded() {

        GA3 = FindObjectOfType<GoogleAnalyticsV3>();

        if (Application.loadedLevelName == "Gamescreen") {

            pauseScreen = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
            collectedOrbs = 0;
            collectedRelics = 0;

            GA3.LogEvent("Game","Game","Game started",1);
        }

        if (Application.loadedLevelName == "ScoreScene") {

            if(managerInstance == this) {
                totalOrbs += collectedOrbs;
                totalRelics += collectedRelics;
                PlayerPrefs.SetInt("Orbs", totalOrbs);
                PlayerPrefs.SetInt("Relics", totalRelics);

                PlayerPrefs.Save();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
        if (Application.loadedLevelName == "MainMenu" && Input.GetMouseButtonDown(0)) {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Application.loadedLevelName == "MainMenu") {
                Application.Quit();
            } else if (Application.loadedLevelName == "Gamescreen" && paused) {
                ReturnToMain();
            } else if (Application.loadedLevelName == "Gamescreen" && !paused) {
                UpdatePauseState();
            }
        }
	}

    public void playerDied() {
        Time.timeScale = 1;
        Application.LoadLevel("ScoreScene");
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

    public int getTotalOrbs() {
        return totalOrbs;
    }

    public void UpdatePauseState() {
        paused = !paused;
        pauseScreen.enabled = !pauseScreen.enabled;
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    public void StartGame() {
        Application.LoadLevel("Gamescreen");
        paused = false;
    }

    public void ReturnToMain() {
        Time.timeScale = 1;
        Application.LoadLevel("MainMenu");
    }

    public IEnumerator updateTimescale(float currentScale, float targetScale) {
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

    public GoogleAnalyticsV3 getAnalytics() {
        return GA3;
    }
}