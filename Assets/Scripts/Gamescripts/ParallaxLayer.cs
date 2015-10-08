using UnityEngine;
using System.Collections;

public class ParallaxLayer : MonoBehaviour {

    private ParallaxController parallaxController;

    public float parallaxSpeedX;
    public float parallaxSpeedY;
    private bool lastGenerated = true;

    void Start() {
        //parallaxController = FindObjectOfType<ParallaxController>();
        //parallaxController.AddLayerToList(this);
    }


    public bool isLastGenerated() {
        return lastGenerated;
    }

    public void setLastGenerated(bool value) {
        lastGenerated = value;

    }
}
