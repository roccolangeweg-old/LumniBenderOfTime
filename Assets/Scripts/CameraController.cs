using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PlayerController player;
    private Vector3 lastPlayerPosition;
    private float distanceToMove;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(player.transform.position.x);
        distanceToMove = player.transform.position.x - lastPlayerPosition.x;
        Debug.Log(distanceToMove);
        this.transform.position = new Vector3(this.transform.position.x + distanceToMove, this.transform.position.y, this.transform.position.z);

        lastPlayerPosition = player.transform.position;

	}
}
