using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxController : MonoBehaviour {

    private List<ParallaxLayer> backgrounds;
    private GameObject gameCamera;
    private GenerationPoint generationPoint;
    private GameObject destructionPoint;

    private Vector3 lastCameraPosition;

    void Awake () {
        backgrounds = new List<ParallaxLayer>();
    }

	// Use this for initialization
	void Start () {
        gameCamera = GameObject.Find("Main Camera");
        generationPoint = FindObjectOfType<GenerationPoint>();
        destructionPoint = GameObject.Find("DestructionPoint");
	}
	
	// Update is called once per frame
	void Update () {

        if (lastCameraPosition.x != 0 || lastCameraPosition.y != 0) {

            float deltaPositionX = gameCamera.transform.position.x - lastCameraPosition.x;
            float deltaPositionY = gameCamera.transform.position.y - lastCameraPosition.y;

            List<ParallaxLayer> layersToDelete = new List<ParallaxLayer>();

            for(int i = 0; i < backgrounds.Count; i++) {

                ParallaxLayer layer = backgrounds[i];

                layer.transform.position = new Vector3(layer.transform.position.x + (deltaPositionX * layer.parallaxSpeedX), layer.transform.position.y + (deltaPositionY * layer.parallaxSpeedY), layer.transform.position.z);

                if(layer.isLastGenerated() && layer.transform.position.x <= generationPoint.transform.position.x) {
                    layer.setLastGenerated(false);

                    GameObject newLayer = (GameObject) Instantiate(layer.gameObject);
                    newLayer.transform.position = new Vector3(layer.transform.position.x + 32.0f, layer.transform.position.y, layer.transform.position.z);
                }

                if(layer.transform.position.x <= destructionPoint.transform.position.x) {
                    layersToDelete.Add(layer);
                } 

            }

            for (int i=0; i < layersToDelete.Count; i++) {
                backgrounds.Remove(layersToDelete[i]);
                Destroy(layersToDelete[i].gameObject);
            }

        }

        lastCameraPosition = gameCamera.transform.position;

	}

    public void AddLayerToList(ParallaxLayer layer) {
        backgrounds.Add(layer);
    }

    public void loadBackgrounds(List<GameObject> newBackgrounds) {

        for (int i=0; i < backgrounds.Count; i++) {
            Destroy(backgrounds[i].gameObject);
        }

        backgrounds = new List<ParallaxLayer>();

        for (int i=0; i < newBackgrounds.Count; i++) {
        
            Instantiate(newBackgrounds[i], new Vector3(gameCamera.transform.position.x, gameCamera.transform.position.y + 1, newBackgrounds[i].transform.position.z + 5), newBackgrounds[i].transform.rotation);
        }
    }
}
