using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class SplashManager : MonoBehaviour {

    public Canvas titleCanvas;
    public Canvas splashCanvas;

	// Use this for initialization
	void Start () {
        StartCoroutine(PlaySplashScreen());
	}

    public void TouchMainMenu() {
        StartCoroutine(GoToMainMenu());
    }

    private IEnumerator GoToMainMenu() {
        titleCanvas.GetComponentInChildren<Image>().GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        Application.LoadLevel("MainScene");
    }

    private IEnumerator PlaySplashScreen() {
        yield return new WaitForSeconds(3f);

        splashCanvas.GetComponentInChildren<Image>().GetComponent<Animator>().enabled = true;
        
        yield return new WaitForSeconds(0.5f * Time.timeScale);

        splashCanvas.enabled = false;
        titleCanvas.GetComponentInChildren<Image>().GetComponent<Animator>().enabled = true;
        
    }

}
