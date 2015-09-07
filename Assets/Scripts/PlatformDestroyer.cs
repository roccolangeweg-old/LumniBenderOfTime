using UnityEngine;
using System.Collections;

public class PlatformDestroyer : MonoBehaviour {

    private GameObject platformDestructionPoint;

	// Use this for initialization
	void Start () {
        platformDestructionPoint = GameObject.Find("PlatformDestructionPoint");
	}
	
	// Update is called once per frame
	void Update () {

	    if (this.transform.position.x < platformDestructionPoint.transform.position.x) {

            //Destroy(this.gameObject);

            this.gameObject.SetActive(false);
        }
	}
}
