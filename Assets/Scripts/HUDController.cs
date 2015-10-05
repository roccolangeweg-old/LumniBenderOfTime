using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class HUDController : MonoBehaviour {

    private Canvas myCanvas;
    private PlayerController player;
    private GameManager gameManager;

    public Text orbText;
    public Text scoreText;
    public Text comboText;


	// Use this for initialization
	void Start () {
        myCanvas = GetComponent<Canvas>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = FindObjectOfType<PlayerController>();

        if (Application.loadedLevelName == "GameScene") {
            StartCoroutine(RemoveControlsAfterStart());
        }
	}

    void Update() {

        orbText.text = gameManager.getCollectedOrbs().ToString();
        scoreText.text = gameManager.getCurrentScore().ToString();
        
        if (gameManager.RoundedCombo() >= 2) {
            comboText.text = gameManager.RoundedCombo().ToString() + "X";
        } else {
            comboText.text = "1X";
        }

    }

    /* PUBLIC METHODS */
    public void PlayerJump() {
        player.Jump();
    }

    public void PlayerAttack() {
        player.Attack();
    }

    public void PlayerTimebend() {
        player.Timebend();
    }

    public void StartGame() {
        gameManager.StartGame();
    }


    /* PRIVATE METHODS */
    private IEnumerator RemoveControlsAfterStart() {
        yield return new WaitForSeconds(3);
        myCanvas.transform.FindChild("Controls").GetComponent<Animator>().SetBool("FadeOut", true);
    }
}
