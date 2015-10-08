using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    private GameManager gameManager;

    public Image thumbnail;

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

        previousOrbs = gameManager.getTotalOrbs() - gameManager.getCollectedOrbs();
       
        collectedOrbTxt.text = gameManager.getCollectedOrbs().ToString();
        enemiesDefeatedTxt.text = gameManager.getEnemiesDefeated().ToString();
        amntOfJumpsTxt.text = gameManager.getCurrentJumps().ToString();
        distanceTxt.text = gameManager.getCurrentDistance().ToString() + " m";
        comboTxt.text = gameManager.getCurrentHighestCombo().ToString() + " x";

        scoreTxt.text = "GAME OVER! YOU SCORED " + gameManager.getCurrentScore().ToString() + " POINTS";
        totalOrbTxt.text = previousOrbs.ToString();

        thumbnail.sprite = Sprite.Create(gameManager.EveryplayThumbnail(), new Rect(0, 0, gameManager.EveryplayThumbnail().width, gameManager.EveryplayThumbnail().height), new Vector2(0.5f, 0.5f));

	}

    void Update() {
        if (gameManager.getTotalOrbs() > previousOrbs) {
            previousOrbs += 1;
            totalOrbTxt.text = previousOrbs.ToString();
        }
    }

    public void ShareReplay() {
        Everyplay.ShowSharingModal();
    }

    public void PlayReplay() {
        Everyplay.PlayLastRecording();
    }
	
    public void ReturnToMainMenu() {
        gameManager.ReturnToMain();
    }

    public void RestartGame() {
        gameManager.StartGame();
    }
}
