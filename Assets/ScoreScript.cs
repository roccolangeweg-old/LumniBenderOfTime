using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    private GameManager gameManager;

    public Text collectedOrb;
    public Text totalOrb;

	// Use this for initialization
	void Start () {

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.getAnalytics().LogEvent("Pause Screen","Start","SceneLoaded",1);

        collectedOrb.text = gameManager.getCollectedOrbs().ToString();
        totalOrb.text = gameManager.getTotalOrbs().ToString();
	}
	
    public void ReturnToMainMenu() {
        gameManager.getAnalytics().LogEvent("Pause Screen","Buttons","MainMenu",1);
        gameManager.ReturnToMain();
    }

    public void RestartGame() {
        gameManager.getAnalytics().LogEvent("Pause Screen","Buttons","Restart",1);
        gameManager.StartGame();
    }
}
