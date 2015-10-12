using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    private PlayerController player;
    private Vector3 lastPlayerPosition;
    private float distanceToMove;

    public Canvas HUDCanvas;

    private List<Image> heartUIs;
    public List<Sprite> heartContainers;

    public int totalHearts;
    private float lastPlayerHealth;



    void Awake() {

        player = FindObjectOfType<PlayerController>();
        transform.position = new Vector3(player.transform.position.x + 2.5f, player.transform.position.y, transform.position.z);

    }

	// Use this for initialization
	void Start () {

        lastPlayerPosition = new Vector3(player.transform.position.x + 2.5f, player.transform.position.y, transform.position.z);

        heartUIs = new List<Image>();

        lastPlayerHealth = player.getTotalHealth();
        totalHearts = Mathf.CeilToInt(lastPlayerHealth);

        for(int i = 0; i < totalHearts; i++) {
            GameObject newHeart = new GameObject("Heart");
            newHeart.transform.SetParent(HUDCanvas.transform);

            Image newImage = (Image) newHeart.AddComponent<Image>();
            heartUIs.Add(newImage);

            newImage.sprite = heartContainers[0];
            newImage.transform.SetParent(GameObject.Find("Health").transform);
            newImage.rectTransform.anchorMax = new Vector2(0.5f,0.5f);
            newImage.rectTransform.anchorMin = new Vector2(0.5f,0.5f);
            newImage.rectTransform.anchoredPosition3D = new Vector3(35 + 35 * i, -30, 0);
            newImage.rectTransform.localScale = new Vector3(0.33f,0.33f,1);
        }
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (player.GetCameraFollow()) {

            distanceToMove = (player.transform.position.x + 2.5f) - lastPlayerPosition.x;
            this.transform.position = new Vector3(this.transform.position.x + distanceToMove, this.transform.position.y, this.transform.position.z);

            lastPlayerPosition = new Vector3(player.transform.position.x + 2.5f, player.transform.position.y, transform.position.z);

            if (player.getCurrentHealth() != lastPlayerHealth) {
                lastPlayerHealth = player.getCurrentHealth();
                updateHearts(lastPlayerHealth);
            }
        }

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
