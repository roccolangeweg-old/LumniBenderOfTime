using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxController : MonoBehaviour {

    private List<ParallaxLayer> backgrounds;
    private GameObject gameCamera;

    private Vector3 lastCameraPosition;

	// Use this for initialization
	void Start () {

        gameCamera = GameObject.Find("Main Camera");
        backgrounds = new List<ParallaxLayer>();

        lastCameraPosition = gameCamera.transform.position;

        for (var i=0; i < transform.childCount; i++) {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if(layer != null) {
                //layer.transform.position = lastCameraPosition;
                backgrounds.Add(layer);
            }
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {

        float deltaPositionX = gameCamera.transform.position.x - lastCameraPosition.x;
        float deltaPositionY = gameCamera.transform.position.y - lastCameraPosition.y;

        for(int i = 0; i < backgrounds.Count; i++) {

            ParallaxLayer layer = backgrounds[i];

            layer.transform.position = new Vector3(layer.transform.position.x + (deltaPositionX * layer.parallaxSpeedX), layer.transform.position.y + (deltaPositionY * layer.parallaxSpeedY), layer.transform.position.z);

        }

        lastCameraPosition = gameCamera.transform.position;

	}
}
