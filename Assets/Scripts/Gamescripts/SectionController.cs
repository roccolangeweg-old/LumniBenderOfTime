using UnityEngine;
using System.Collections;

public class SectionController : MonoBehaviour {

    public float sectionWidth;

    private GameObject destructionPoint;

	// Use this for initialization
	void Start () {
        destructionPoint = GameObject.Find("DestructionPoint");
	}
	
	// Update is called once per frame
	void Update () {
	    
        if (transform.position.x < destructionPoint.transform.position.x) {
            ObjectPooler.instance.AddToPool(gameObject);
        }
	}

    public float getSectionWidth() {
        return this.sectionWidth;
    }
}
