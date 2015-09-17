using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxController : MonoBehaviour {

    private List<ParallaxLayer> backgrounds;
    private GameObject gameCamera;
    private GenerationPoint generationPoint;

    private Vector3 lastCameraPosition;

    void Awake () {
        backgrounds = new List<ParallaxLayer>();
    }

	// Use this for initialization
	void Start () {
        gameCamera = GameObject.Find("Main Camera");
        generationPoint = FindObjectOfType<GenerationPoint>();
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (lastCameraPosition.x != 0 || lastCameraPosition.y != 0) {

            float deltaPositionX = gameCamera.transform.position.x - lastCameraPosition.x;
            float deltaPositionY = gameCamera.transform.position.y - lastCameraPosition.y;

            for(int i = 0; i < backgrounds.Count; i++) {

                ParallaxLayer layer = backgrounds[i];

                layer.transform.position = new Vector3(layer.transform.position.x + (deltaPositionX * layer.parallaxSpeedX), layer.transform.position.y + (deltaPositionY * layer.parallaxSpeedY), layer.transform.position.z);

                if(layer.isLastGenerated() && layer.transform.position.x < generationPoint.transform.position.x) {
                    layer.setLastGenerated(false);

                    GameObject newLayer = (GameObject) Instantiate(layer.gameObject);
                    newLayer.transform.position = new Vector3(layer.transform.position.x + 32, layer.transform.position.y, layer.transform.position.z);
                }

            }

        }

        lastCameraPosition = gameCamera.transform.position;

	}

    public void AddLayerToList(ParallaxLayer layer) {
        Debug.Log(backgrounds);
        backgrounds.Add(layer);
    }
}
