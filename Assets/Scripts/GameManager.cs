using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

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
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (Application.loadedLevelName == "MainMenu" && Input.GetMouseButtonDown(0)) {
            Application.LoadLevel("Gamescreen");
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Application.loadedLevelName == "MainMenu") {
                Application.Quit();
            } else {
                Application.LoadLevel("MainMenu");
            }
        }
	}

    public void playerDied() {
        Debug.Log("GAME OVER");
        Application.LoadLevel("MainMenu");
    }
}
