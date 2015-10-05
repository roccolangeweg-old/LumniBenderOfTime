using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Collections.Generic;

public class TimebendController : MonoBehaviour {

    private GameManager gameManager;

    private PlayerController player;
    private Canvas myCanvas;
    private Slider timeSlider;

    private bool timebendEnabled;
    private bool sequencePlaying;
    private int remainingTaps;

    public GameObject markerObject;
    public float timeMultiplier;

    private List<GameObject> targets;
    private List<Image> createdMarkers;

    public List<AudioClip> targetHitSounds;
    public AudioClip timebendAttack;

	// Use this for initialization
	void Start() {
        myCanvas = GetComponent<Canvas>();
        player = FindObjectOfType<PlayerController>();
        timeSlider = GetComponentInChildren<Slider>();

        gameManager = GameManager.instance;

        timebendEnabled = false;
	}

    void Update() {
        if (timebendEnabled && Time.timeScale > 0) {

            timeSlider.value -= (Time.deltaTime / Time.timeScale) * timeMultiplier;

            if ((timeSlider.value <= 0 || remainingTaps == 0) && !sequencePlaying) {
                PlayTimebendSequence();

            }
        }

    }

    /* PUBLIC METHODS */
    public void EnableTimebendMode() {

        if (!timebendEnabled) {
            StartCoroutine(gameManager.UpdateTimescale(0.00001f));

            timebendEnabled = true;
            myCanvas.enabled = true;
            remainingTaps = 5;
            timeSlider.value = 100;

            targets = new List<GameObject>();
            createdMarkers = new List<Image>();

            sequencePlaying = false;
            
            UpdateHUD();
        }
    }
    
    public void DisableTimebendMode() {
        timebendEnabled = false;

        GetComponent<AudioSource>().PlayOneShot(timebendAttack);

        StartCoroutine(gameManager.UpdateTimescale(1f));

        targets.Clear();
    }
	
    public void Tap() {

        if (timebendEnabled) {

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            RaycastHit2D hit = Physics2D.Raycast(touchPosition, (Input.GetTouch(0).position));

            if (hit.collider && hit.collider.tag == "Enemy" && hit.collider.gameObject.layer != LayerMask.NameToLayer("DeadEnemies")){

                targets.Add(hit.collider.gameObject);

                GameObject marker = (GameObject) Instantiate(markerObject);
                Image markerImage = marker.GetComponent<Image>();

                markerImage.transform.SetParent(myCanvas.transform, false);
                markerImage.rectTransform.anchoredPosition = new Vector2(0, 20);
                markerImage.rectTransform.pivot = new Vector2(0.5f,0.5f);
                
                Vector2 pos = hit.collider.transform.position; 
                Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos); 

                markerImage.rectTransform.anchorMin = viewportPoint;  
                markerImage.rectTransform.anchorMax = viewportPoint;

                markerImage.rectTransform.localScale = new Vector3(2f,2f,2f);
                createdMarkers.Add(markerImage);

                remainingTaps--;

                GetComponent<AudioSource>().PlayOneShot(TargetSound(5 - remainingTaps));

                UpdateHUD();
            }
        }

    }

    public AudioClip TargetSound(int position) {
        if (position > targetHitSounds.Count) {
            return targetHitSounds [targetHitSounds.Count];
        } else {
            return targetHitSounds [position];
        }
    }


    /* PRIVATE METHODS */
    private void UpdateHUD() {

        float markersWidth = ((50 * remainingTaps) + (25 * (remainingTaps - 1))) / 2;
        float currentPositionX = markersWidth * -1;

        GameObject container = GameObject.Find("MarkerContainer");

        for (int i = 0; i < container.transform.childCount; i++) {
            Destroy(container.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < remainingTaps; i++) {
            GameObject marker = (GameObject) Instantiate(markerObject);
            Image markerImage = marker.GetComponent<Image>();

            markerImage.transform.SetParent(container.transform, false);
            markerImage.rectTransform.anchoredPosition = new Vector2(currentPositionX, markerImage.rectTransform.anchoredPosition.y);
            markerImage.rectTransform.anchorMax = new Vector2(0.5f,0f);
            markerImage.rectTransform.anchorMin = new Vector2(0.5f,0f);
            markerImage.rectTransform.localScale = new Vector3(1f,1f,1f);

            currentPositionX += 75;
        }

    }

    private void PlayTimebendSequence() {
        sequencePlaying = true;
        myCanvas.enabled = false;

        for(int i = 0; i < createdMarkers.Count; i++) {
            Destroy(createdMarkers[i].gameObject);
        }

        player.TimebendAttack(targets);
    }

}
