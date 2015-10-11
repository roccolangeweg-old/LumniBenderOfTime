using UnityEngine;
using System.Collections;

using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    private GoogleAnalyticsV3 GA3;
    private PlayerController player;

    private TimebendController tbController;

    private bool paused;
    private float healthMultiplier;

    private Canvas pauseScreen;
    private float lastTimescale;

    private bool loadingGameScene;
    private AsyncOperation gameLoader;

    /* stuff to save */
    private int collectedOrbs;
    private int totalOrbs;

    private int currentJumps;
    private int totalJumps;

    private float currentCombo;
    private int highestCombo;
    private int currentHighestCombo;

    private int totalEnemiesDefeated;
    private int enemiesDefeated;

    private int totalRelics;
    private int collectedRelics;

    private int totalDistance;
    private int currentDistance;
    private float startingDistance;

    private float currentScore;
    private int highScore;

    public float multiplierPercentage;

    private Texture2D everyplayThumbnail;
   
    //Awake is always called before any Start functions
    void Awake() {
        /* check if gamemanager already exists */
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            /* enforce Singleton, only 1 game manager may exist */
            Destroy(gameObject);
        }    

        GA3 = FindObjectOfType<GoogleAnalyticsV3>();

        Application.targetFrameRate = 30;
        Time.timeScale = 1f;

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
           
	// Use this for initialization
	void Start () {
        healthMultiplier = 1;


        if (PlayerPrefs.GetInt("TotalGames") < 1) {
            PlayerPrefs.SetInt("ShowHelpOnStartup", 1);
        }

        totalOrbs = PlayerPrefs.GetInt("Orbs");
        totalRelics = PlayerPrefs.GetInt("Relics");
        totalDistance = PlayerPrefs.GetInt("TotalDistance");
        highestCombo = PlayerPrefs.GetInt("HighestCombo");
        totalEnemiesDefeated = PlayerPrefs.GetInt("EnemiesDefeated");
        totalJumps = PlayerPrefs.GetInt("Jumps");

        highScore = PlayerPrefs.GetInt("Highscore");

        player = FindObjectOfType<PlayerController>();
        tbController = FindObjectOfType<TimebendController>();

        everyplayThumbnail = new Texture2D(250, 130, TextureFormat.ARGB32, false);
        everyplayThumbnail.wrapMode = TextureWrapMode.Clamp;

        Everyplay.SetTargetFPS(30);
        Everyplay.SetThumbnailTargetTexture(everyplayThumbnail);


	}

    void OnLevelWasLoaded() {

        GA3 = FindObjectOfType<GoogleAnalyticsV3>();
        GA3.LogScreen(Application.loadedLevelName);

        loadingGameScene = false;
        gameLoader = new AsyncOperation();

        if (Application.loadedLevelName == "GameScene") {

            if(Everyplay.IsRecordingSupported()) {
                Everyplay.StartRecording();
                StartCoroutine(TakeThumbnail());
            }

            pauseScreen = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
            player = FindObjectOfType<PlayerController>();
            tbController = FindObjectOfType<TimebendController>();

            startingDistance = player.transform.position.x;

            collectedOrbs = 0;
            collectedRelics = 0;

            currentJumps = 0;
            currentCombo = 1;
            currentHighestCombo = 0;

            enemiesDefeated = 0;
            currentScore = 0;

        } else if (Application.loadedLevelName == "ScoreScene") {

            if (instance == this) {

                PlayerPrefs.SetInt("TotalGames", PlayerPrefs.GetInt("TotalGames") + 1);

                Dictionary<string, object> stats = new Dictionary<string, object>();

                stats["score"] = Mathf.RoundToInt(currentScore);
                stats["orbs"] = collectedOrbs;
                stats["relics"] = collectedRelics;
                stats["jumps"] = currentJumps;
                stats["enemies"] = enemiesDefeated;
                stats["distance"] = currentDistance.ToString();
                stats["max_combo"] = currentHighestCombo.ToString();

                Everyplay.SetMetadata(stats);
                Everyplay.StopRecording();

				//ShowAd ();

                totalOrbs += collectedOrbs;
                totalRelics += collectedRelics;
                totalJumps += currentJumps;
                totalEnemiesDefeated += enemiesDefeated;
                totalDistance += currentDistance;

                if(Mathf.RoundToInt(currentScore) > highScore) {
                    highScore = Mathf.RoundToInt(currentScore);
                    PlayerPrefs.SetInt("Highscore", highScore);
                }

				Analytics.CustomEvent("Results", stats);

                if (highestCombo < currentHighestCombo) {
                    highestCombo = currentHighestCombo;
                }

                PlayerPrefs.SetInt("Orbs", totalOrbs);
                PlayerPrefs.SetInt("Relics", totalRelics);
                PlayerPrefs.SetInt("HighestCombo", highestCombo);
                PlayerPrefs.SetInt("TotalEnemiesDefeated", totalEnemiesDefeated);
                PlayerPrefs.SetInt("TotalDistance", totalDistance);
                PlayerPrefs.SetInt("Highscore", highScore);

                PlayerPrefs.Save();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Application.loadedLevelName != "GameScene" && !loadingGameScene) {
            StartCoroutine(LoadGameScene());
            loadingGameScene = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Application.loadedLevelName == "MainScene") {
                Application.Quit();
            } else if (Application.loadedLevelName == "GameScene" && paused) {
                ReturnToMain();
            } else if (Application.loadedLevelName == "GameScene" && !paused) {
                Debug.Log(Time.timeScale);
                UpdatePauseState();
            }
        }

        if(Input.GetKeyDown(KeyCode.A)) {
            player.Timebend();
        }
        
	}

    public bool ShowHelpOnStartup() {
        return PlayerPrefs.GetInt("ShowHelpOnStartup")==1?true:false;
    }

    public void playerDied() {
        currentDistance = (int) Mathf.Round(player.transform.position.x - startingDistance);
        Time.timeScale = 1;
        Application.LoadLevel("ScoreScene");
    }

    public float getHealthMultiplier() {
        return healthMultiplier;
    }

    /* stat update methods */
    public void addOrbs(int amount) {
        collectedOrbs += amount;
        addScore(amount);
    }

    public void addScore(float amount) {
        if (!paused) {
            currentScore += amount * ScoreMultiplier();
        }
    }

    public void addDefeatedEnemy() {
        enemiesDefeated++;
        UpdateComboMeter();
    }

    public void addJump() {
        currentJumps++;
    }

    /* get stats for this run */
    public int getCollectedOrbs() {
        return collectedOrbs;
    }

    public int getCurrentDistance() {
        return currentDistance;
    }

    public int getCurrentHighestCombo() {
        return currentHighestCombo;
    }

    public int getCurrentJumps() {
        return currentJumps;
    }

    public int getEnemiesDefeated() {
        return enemiesDefeated;
    }

    public int getCurrentScore() {
        return Mathf.RoundToInt(currentScore);
    }

    /* totals for stats screen */
    public int getTotalOrbs() {
        return totalOrbs;
    }

    public int getTotalDistance() {
        return totalDistance;
    }

    public int getHighestCombo() {
        return highestCombo;
    }

    public int getTotalJumps() {
        return totalJumps;
    }

    public int getTotalEnemiesDefeated() {
        return totalEnemiesDefeated;
    }

    public void UpdatePauseState() {
        paused = !paused;
        pauseScreen.enabled = !pauseScreen.enabled;

        if (pauseScreen.enabled) {
            lastTimescale = Time.timeScale;
            Time.timeScale = 0;
            Everyplay.PauseRecording();
        } else {
            Time.timeScale = lastTimescale;
            Everyplay.ResumeRecording();
        }
            
    }

    public void StartGame() {
        if (gameLoader != null) {
            gameLoader.allowSceneActivation = true;
            paused = false;
        }
    }

    public void ReturnToMain() {
        Time.timeScale = 1;
        Everyplay.StopRecording();
        Application.LoadLevel("MainScene");
    }

    public IEnumerator UpdateTimescale(float targetScale) {

        float currentScale = Time.timeScale;

        if (currentScale < targetScale) {
            while (currentScale < targetScale) {

                if(currentScale + 0.2f > targetScale) {
                    currentScale = targetScale;
                } else {
                    currentScale += 0.2f;
                }

                Time.timeScale = currentScale;
                yield return new WaitForEndOfFrame();
            }
        } else {
            while (currentScale > targetScale) {

                if(currentScale - 0.2f < targetScale) {
                    currentScale = targetScale;
                } else {
                    currentScale -= 0.2f;
                }

                Time.timeScale = currentScale;
                yield return new WaitForEndOfFrame();
            }
        }
    }

	public void ShowAd() {
        if (Random.Range(1, 4) == 1) {

            if (Advertisement.IsReady()) {
                //Advertisement.Show();
            }

        }
	}

    private float ScoreMultiplier() {
        return Mathf.Floor(currentCombo) * multiplierPercentage;
    }

    private void UpdateComboMeter() {
        currentCombo += 1 / (RoundedCombo() + 1);

        if (RoundedCombo() > currentHighestCombo) {
            currentHighestCombo = (int) RoundedCombo();
        }
    }

    private IEnumerator LoadGameScene() {
        gameLoader = Application.LoadLevelAsync("GameScene");
        gameLoader.allowSceneActivation = false;

        yield return gameLoader.isDone;
    }



    public float RoundedCombo() {
        return Mathf.Floor(currentCombo);
    }

    public void ResetCombo() {
        currentCombo = 1;
    }

    public TimebendController GetTBController() {
        return tbController;
    }

    private IEnumerator TakeThumbnail() {
        yield return new WaitForSeconds(5 * Time.timeScale); 
        Everyplay.TakeThumbnail();    
    }

    public Texture2D EveryplayThumbnail() {
        return everyplayThumbnail;
    }

    public int GetHighscore() {
        return highScore;
    }

}