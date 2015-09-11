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

        collectedOrb.text = gameManager.getCollectedOrbs().ToString();
        totalOrb.text = gameManager.getTotalOrbs().ToString();
	}
	
    public void ReturnToMainMenu() {
        gameManager.ReturnToMain();
    }

    public void RestartGame() {
        gameManager.StartGame();
    }
}
