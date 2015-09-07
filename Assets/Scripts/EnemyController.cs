using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int orbsRewarded;
    public float baseHealth;
    public bool isAerialType;
    public float moveSpeed;

    private Rigidbody2D myRigidbody;
    private Animator myAnimator;

	// Use this for initialization
	void Start () {
        myRigidbody = this.GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        myRigidbody.velocity = new Vector2(-moveSpeed, myRigidbody.velocity.y);
        myAnimator.SetFloat("Speed", Mathf.Abs(myRigidbody.velocity.x));
	}
}
