using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TimebendImageScript : MonoBehaviour {

    private int percentage;
    private Image myImage;
    public List<Sprite> percSprites;

	// Use this for initialization
	void Start () {
        percentage = 0;
        myImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {

        int spriteNumber = Mathf.FloorToInt(percentage / 2) - 1;
        if(spriteNumber < 0) { spriteNumber = 0; }
        myImage.sprite = percSprites [spriteNumber];

	}

    public void setPercentage(int percent) {
        percentage = percent;
    }
}
