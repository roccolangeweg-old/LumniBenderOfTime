using UnityEngine;
using System.Collections;

using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager gmInstance = null;

    private GoogleAnalyticsV3 GA3;
    private PlayerController player;

    private bool paused;
    private float healthMultiplier;

    private int bonusTargets;

    private Canvas pauseScreen;

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
   
    //Awake is always called before any Start functions
    void Awake() {
        /* check if gamemanager already exists */
        if (gmInstance == null) {
            gmInstance = this;
        } else if (gmInstance != this) {
            /* enforce Singleton, only 1 game manager may exist */
            Destroy(gameObject);
        }    

        GA3 = FindObjectOfType<GoogleAnalyticsV3>();

        Application.targetFrameRate = 30;

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
           
	// Use this for initialization
	void Start () {
        healthMultiplier = 1;

        totalOrbs = PlayerPrefs.GetInt("Orbs");
        totalRelics = PlayerPrefs.GetInt("Relics");
        totalDistance = PlayerPrefs.GetInt("TotalDistance");
        highestCombo = PlayerPrefs.GetInt("HighestCombo");
        totalEnemiesDefeated = PlayerPrefs.GetInt("EnemiesDefeated");
        totalJumps = PlayerPrefs.GetInt("Jumps");

        player = FindObjectOfType<PlayerController>();
	}

    void OnLevelWasLoaded() {

        GA3 = FindObjectOfType<GoogleAnalyticsV3>();
        GA3.LogScreen(Application.loadedLevelName);

        if (Application.loadedLevelName == "GameScene") {

            pauseScreen = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
            player = FindObjectOfType<PlayerController>();
            startingDistance = player.transform.position.x;

            collectedOrbs = 0;
            collectedRelics = 0;

            currentJumps = 0;
            currentCombo = 1;
            currentHighestCombo = 0;

            enemiesDefeated = 0;
            currentScore = 0;


        } else if (Application.loadedLevelName == "ScoreScene") {

            if (gmInstance == this) {

				ShowAd ();

                totalOrbs += collectedOrbs;
                totalRelics += collectedRelics;
                totalJumps += currentJumps;
                totalEnemiesDefeated += enemiesDefeated;
                totalDistance += currentDistance;

                if(Mathf.RoundToInt(currentScore) > highScore) {
                    highScore = Mathf.RoundToInt(currentScore);
                }

				Analytics.CustomEvent("Results", new Dictionary<string, object>
				{
					{ "score", currentScore },
					{ "orbs", collectedOrbs },
					{ "relics", collectedRelics },
					{ "jumps", currentJumps },
					{ "enemies", enemiesDefeated },
					{ "distance", currentDistance }

				});

                GA3.LogEvent("Stats","All","Score", Mathf.RoundToInt(currentScore) );
                GA3.LogEvent("Stats","All","Orbs Collected",collectedOrbs);
                GA3.LogEvent("Stats","All","Relics Collected",collectedRelics);
                GA3.LogEvent("Stats","All","Jump Count", currentJumps);
                GA3.LogEvent("Stats","All","Defeated Enemies", enemiesDefeated);
                GA3.LogEvent("Stats","All","Distance Travelled", currentDistance);

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

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Application.loadedLevelName == "MainScene") {
                Application.Quit();
            } else if (Application.loadedLevelName == "GameScene" && paused) {
                ReturnToMain();
            } else if (Application.loadedLevelName == "GameScene" && !paused) {
                UpdatePauseState();
            }
        }

        if(Input.GetKeyDown(KeyCode.A)) {
            Debug.Log(player);
            player.Timebend();
        }
        
	}

    public int MaxTBTargets(int targets) {
        return targets + bonusTargets;
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
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    public void StartGame() {
        Application.LoadLevel("GameScene");
        paused = false;
    }

    public void ReturnToMain() {
        Time.timeScale = 1;
        Application.LoadLevel("MainScene");
    }

    public IEnumerator UpdateTimescale(float targetScale) {

        float currentScale = Time.timeScale;

        if (currentScale < targetScale) {
            while (currentScale < targetScale) {

                if(currentScale + 0.1f > targetScale) {
                    currentScale = targetScale;
                } else {
                    currentScale += 0.1f;
                }

                Time.timeScale = currentScale;
                yield return new WaitForEndOfFrame();
            }
        } else {
            while (currentScale > targetScale) {

                if(currentScale - 0.1f < targetScale) {
                    currentScale = targetScale;
                } else {
                    currentScale -= 0.1f;
                }

                Time.timeScale = currentScale;
                yield return new WaitForEndOfFrame();
            }
        }
    }

	public void ShowAd() {
        if (Random.Range(1, 5) == 1) {

            if (Advertisement.IsReady()) {
                Advertisement.Show();
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
    
        StartCoroutine(StartComboExpire());
    }

    public float RoundedCombo() {
        return Mathf.Floor(currentCombo);
    }

    public void ResetCombo() {
        currentCombo = 1;
    }

    public GoogleAnalyticsV3 getAnalytics() {
        return GA3;
    }

    private IEnumerator StartComboExpire() {
        float currentComboStart = currentCombo;

        yield return new WaitForSeconds(5);

        if (currentCombo == currentComboStart) {
            currentCombo = 1;
        }
    }

}