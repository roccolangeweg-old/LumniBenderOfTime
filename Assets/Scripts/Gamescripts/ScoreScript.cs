using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    private GameManager gameManager;

    public Text collectedOrbTxt;
    public Text enemiesDefeatedTxt;
    public Text amntOfJumpsTxt;
    public Text distanceTxt;
    public Text comboTxt;

    public Text scoreTxt;

    public Text totalOrbTxt;

    private int previousOrbs;

	// Use this for initialization
	void Start () {

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.getAnalytics().LogEvent("Stats","Score","Orbs Collected",gameManager.getCollectedOrbs());

        previousOrbs = gameManager.getTotalOrbs() - gameManager.getCollectedOrbs();    

        collectedOrbTxt.text = gameManager.getCollectedOrbs().ToString();
        enemiesDefeatedTxt.text = gameManager.getEnemiesDefeated().ToString();
        amntOfJumpsTxt.text = gameManager.getCurrentJumps().ToString();
        distanceTxt.text = gameManager.getCurrentDistance().ToString() + " m";
        comboTxt.text = gameManager.getCurrentHighestCombo().ToString() + " x";

        scoreTxt.text = gameManager.getCurrentScore().ToString();

        totalOrbTxt.text = previousOrbs.ToString();
	}

    void Update() {
        if (gameManager.getTotalOrbs() > previousOrbs) {
            previousOrbs += 1;
            totalOrbTxt.text = previousOrbs.ToString();
        }
    }
	
    public void ReturnToMainMenu() {
        gameManager.getAnalytics().LogEvent("Scorescreen","Action","Return to main menu",1);
        gameManager.ReturnToMain();
    }

    public void RestartGame() {
        gameManager.getAnalytics().LogEvent("Scorescreen","Action","Restart game",1);
        gameManager.StartGame();
    }
}
