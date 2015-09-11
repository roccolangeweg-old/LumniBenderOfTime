using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    private GameManager gameManager;

    private PlayerController player;
    private Vector3 lastPlayerPosition;
    private float distanceToMove;

    public Canvas HUDCanvas;

    private List<Image> heartUIs;
    public List<Sprite> heartContainers;

    public int totalHearts;
    private float lastPlayerHealth;


	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        heartUIs = new List<Image>();

        lastPlayerHealth = player.getTotalHealth();
        totalHearts = Mathf.CeilToInt(lastPlayerHealth);

        for(int i = 0; i < totalHearts; i++) {
            GameObject newHeart = new GameObject("Heart");
            newHeart.transform.SetParent(HUDCanvas.transform);

            Image newImage = (Image) newHeart.AddComponent<Image>();
            heartUIs.Add(newImage);

            newImage.sprite = heartContainers[0];
            newImage.rectTransform.anchorMax = new Vector2(0,1);
            newImage.rectTransform.anchorMin = new Vector2(0,1);
            newImage.rectTransform.anchoredPosition3D = new Vector3(35 + 35 * i, -30.5f, 0);
            newImage.rectTransform.localScale = new Vector3(0.33f,0.33f,1);
        }
    }
	
	// Update is called once per frame
	void Update () {
        distanceToMove = player.transform.position.x - lastPlayerPosition.x;
        this.transform.position = new Vector3(this.transform.position.x + distanceToMove, this.transform.position.y, this.transform.position.z);

        lastPlayerPosition = player.transform.position;

        if (player.getCurrentHealth() != lastPlayerHealth) {
            lastPlayerHealth = player.getCurrentHealth();
            updateHearts(lastPlayerHealth);
        }

        Text orbText = HUDCanvas.GetComponentInChildren<Text>();
        orbText.text = gameManager.getCollectedOrbs().ToString();

	}

    public void updateHearts(float currentHealth) {

        for(int i = 0; i < totalHearts; i++) {

            Sprite selectedSprite;

            if (currentHealth - i >= 1) {
                selectedSprite = heartContainers[0];
            } else if (currentHealth - i <= 0) {
                selectedSprite = heartContainers[2];
            } else {
                selectedSprite = heartContainers[1];
            }

            heartUIs[i].GetComponent<Image>().sprite = selectedSprite;
        }
    
    }
}
