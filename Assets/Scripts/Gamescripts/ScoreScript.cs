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
        gameManager.getAnalytics().LogEvent("Statistics","Score","Orbs Collected",gameManager.getCollectedOrbs());

        collectedOrb.text = gameManager.getCollectedOrbs().ToString();
        totalOrb.text = gameManager.getTotalOrbs().ToString();
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
