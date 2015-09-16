using UnityEngine;
using System.Collections;

public class HarmlessBats : MonoBehaviour {

    private float upwardDirection;

	// Use this for initialization
	void Start () {
        upwardDirection = Random.Range(3f,5f);
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(5, upwardDirection);
	}
}
