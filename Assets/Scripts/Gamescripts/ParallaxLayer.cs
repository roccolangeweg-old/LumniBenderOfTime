using UnityEngine;
using System.Collections;

public class ParallaxLayer : MonoBehaviour {

    public float parallaxSpeedX;
    public float parallaxSpeedY;
    private bool lastGenerated = true;

    public bool isLastGenerated() {
        return lastGenerated;
    }

    public void setLastGenerated(bool value) {
        lastGenerated = value;

    }
}
